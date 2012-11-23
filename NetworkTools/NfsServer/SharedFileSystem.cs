﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using FileID = System.UInt64;

namespace Marler.NetworkTools
{
    public class ShareDirectory
    {
        public readonly String localShareDirectory;
        public readonly String shareName;
        public ShareObject shareObject;
        public ShareDirectory(String localShareDirectory, String shareName)
        {
            if (!NfsPath.IsValidUnixFileName(shareName))
                throw new ArgumentException(String.Format("The share name you provided '{0}' is not valid (cannot have '/')", shareName));

            this.localShareDirectory = localShareDirectory;
            this.shareName = shareName;
        }
        public override String ToString()
        {
            return (shareObject == null) ?
                String.Format("LocalDirectory '{0}' ShareName '{1}'", localShareDirectory, shareName) :
                shareObject.ToString();
        }
    }
    public class SharedFileSystem
    {
        private readonly IFileIDsAndHandlesDictionary filesDictionary;
        public readonly IPermissions permissions;

        private readonly ShareDirectory[] shareDirectories;

        private readonly Dictionary<Byte[], ShareObject> shareObjectsByHandle;
        private readonly Dictionary<String, ShareObject> shareObjectsByLocalPath;

        public SharedFileSystem(IFileIDsAndHandlesDictionary filesDictionary, IPermissions permissions, ShareDirectory[] shareDirectories)
        {
            this.filesDictionary = filesDictionary;
            this.permissions = permissions;

            this.shareDirectories = shareDirectories;
            
            shareObjectsByHandle = new Dictionary<Byte[], ShareObject>(filesDictionary);
            shareObjectsByLocalPath = new Dictionary<String, ShareObject>();
            for (int i = 0; i < shareDirectories.Length; i++)
            {
                ShareDirectory shareDirectory = shareDirectories[i];
                ShareObject shareObject = CreateNewShareObject(Nfs3Procedure.FileType.Directory, shareDirectory.localShareDirectory, shareDirectory.shareName);
                if (shareObject == null)
                {
                    throw new DirectoryNotFoundException(String.Format(
                        "You are trying to share local directory '{0}', but it either does not exist or is not a directory", shareDirectory.localShareDirectory));
                }
                shareDirectory.shareObject = shareObject;
            }
        }
        private void DisposeShareObject(ShareObject shareObject)
        {
            if (NfsServerLog.sharedFileSystemLogger != null)
                NfsServerLog.sharedFileSystemLogger.WriteLine("[SharedFileSystem] Disposing Share Object: {0}", shareObject);

            filesDictionary.Dispose(shareObject.fileID);
            shareObjectsByLocalPath.Remove(shareObject.localPathAndName);
            shareObjectsByHandle.Remove(shareObject.fileHandleBytes);
        }

        private ShareObject CreateNewShareObject(Nfs3Procedure.FileType fileType, String localPathAndName, String shareName)
        {
            FileID newFileID;
            Byte[] newFileHandle = filesDictionary.NewFileHandle(out newFileID);

            ShareObject shareObject = new ShareObject(fileType, newFileID, newFileHandle, localPathAndName, shareName);
            shareObjectsByLocalPath.Add(shareObject.localPathAndName, shareObject);
            shareObjectsByHandle.Add(shareObject.fileHandleBytes, shareObject);

            if (NfsServerLog.sharedFileSystemLogger != null)
                NfsServerLog.sharedFileSystemLogger.WriteLine("[SharedFileSystem] New Share Object: {0}", shareObject);
            return shareObject;
        }

        public void UpdateShareObjectPathAndName(ShareObject shareObject, String newLocalPathAndName, String newName)
        {
            // Dispose share object at new location
            ShareObject overwriteShareObject;
            if (shareObjectsByLocalPath.TryGetValue(newLocalPathAndName, out overwriteShareObject))
            {
                DisposeShareObject(overwriteShareObject);
            }

            // Update share object with new location
            String oldLocalPathAndName = shareObject.localPathAndName;
            shareObjectsByLocalPath.Remove(shareObject.localPathAndName);

            shareObject.UpdatePathAndName(newLocalPathAndName, newName);
            shareObjectsByLocalPath.Add(newLocalPathAndName, shareObject);

            if (NfsServerLog.sharedFileSystemLogger != null)
                NfsServerLog.sharedFileSystemLogger.WriteLine("[SharedFileSystem] Updated Share Object: '{0}' to '{1}'", oldLocalPathAndName, newLocalPathAndName);
        }


        public Nfs3Procedure.Status RemoveFileOrDirectory(String parentDirectory, String name)
        {
            String localPathAndName = NfsPath.LocalCombine(parentDirectory, name);

            ShareObject shareObject;
            if (shareObjectsByLocalPath.TryGetValue(localPathAndName, out shareObject))
            {
                DisposeShareObject(shareObject);
            }

            if (File.Exists(localPathAndName))
            {
                File.Delete(localPathAndName);
                return Nfs3Procedure.Status.Ok;
            }
            
            if (Directory.Exists(localPathAndName))
            {
                Directory.Delete(localPathAndName);
                return Nfs3Procedure.Status.Ok;
            }

            return Nfs3Procedure.Status.ErrorNoSuchFileOrDirectory;
        }

        public Nfs3Procedure.Status Move(ShareObject oldParentShareObject, String oldName,
            ShareObject newParentShareObject, String newName)
        {
            Nfs3Procedure.Status status;


            status = newParentShareObject.CheckStatus();
            if(status != Nfs3Procedure.Status.Ok) 
            {
                DisposeShareObject(newParentShareObject);
                return status;
            }

            status = oldParentShareObject.CheckStatus();
            if (status != Nfs3Procedure.Status.Ok)
            {
                DisposeShareObject(oldParentShareObject);
                return status;
            }


            
            //
            // Get Old Share Object
            //
            String oldLocalPathAndName = NfsPath.LocalCombine(oldParentShareObject.localPathAndName, oldName);

            ShareObject shareObject;
            if (!shareObjectsByLocalPath.TryGetValue(oldLocalPathAndName, out shareObject))
                return Nfs3Procedure.Status.ErrorNoSuchFileOrDirectory;

            status = shareObject.CheckStatus();
            if(status != Nfs3Procedure.Status.Ok) 
            {
                DisposeShareObject(shareObject);
                return status;
            }

            //
            // Move
            //
            String newLocalPathAndName = NfsPath.LocalCombine(newParentShareObject.localPathAndName, newName);
            Nfs3Procedure.FileType fileType = shareObject.fileType;

            if(Directory.Exists(newLocalPathAndName))
            {
                if(shareObject.fileType != Nfs3Procedure.FileType.Directory)
                    return Nfs3Procedure.Status.ErrorAlreadyExists;

                try
                {
                    Directory.Delete(newLocalPathAndName);
                }
                catch (IOException)
                {
                    return Nfs3Procedure.Status.ErrorDirectoryNotEmpty; // The directory is not empty
                }

                Directory.Move(oldLocalPathAndName, newLocalPathAndName);
            }
            else if (File.Exists(newLocalPathAndName))
            {
                if (shareObject.fileType != Nfs3Procedure.FileType.Regular)
                    return Nfs3Procedure.Status.ErrorAlreadyExists;

                File.Move(oldLocalPathAndName, newLocalPathAndName);
            }
            else
            {
                if (shareObject.fileType == Nfs3Procedure.FileType.Regular)
                {
                    File.Move(oldLocalPathAndName, newLocalPathAndName);
                }
                else if (shareObject.fileType == Nfs3Procedure.FileType.Directory)
                {
                    Directory.Move(oldLocalPathAndName, newLocalPathAndName);
                }
                else
                {
                    return Nfs3Procedure.Status.ErrorInvalidArgument;
                }
            }


            //
            // Update the share object and return
            //
            UpdateShareObjectPathAndName(shareObject, newLocalPathAndName, newName);
            shareObject.RefreshFileAttributes(permissions);
            status = shareObject.CheckStatus();
            if (status != Nfs3Procedure.Status.Ok)
            {
                DisposeShareObject(shareObject);
            }
            return status;
        }


        public Nfs3Procedure.Status TryGetSharedObject(Byte[] handle, out ShareObject shareObject)
        {
            if (!shareObjectsByHandle.TryGetValue(handle, out shareObject))
            {
                if (NfsServerLog.sharedFileSystemLogger != null)
                    NfsServerLog.sharedFileSystemLogger.WriteLine("[SharedFileSystem] [Warning] File handle not found in dictionary: {0}", BitConverter.ToString(handle));
                return Nfs3Procedure.Status.ErrorBadHandle;
            }

            Nfs3Procedure.Status status = shareObject.CheckStatus();
            if (status != Nfs3Procedure.Status.Ok) DisposeShareObject(shareObject);
            return status;
        }

        public Nfs3Procedure.Status TryGetSharedDirectory(String shareName, out ShareObject shareObject)
        {
            if (shareName[0] == '/')
            {
                shareName = shareName.Substring(1);
            }
            // if (shareName.Contains('/'))
            if (shareName.Contains("/"))
            {
                shareObject = null;
                return Nfs3Procedure.Status.ErrorNoSuchFileOrDirectory;
            }

            for (int i = 0; i < shareDirectories.Length; i++)
            {
                ShareDirectory shareDirectory = shareDirectories[i];
                if (shareName.Equals(shareDirectory.shareName))
                {
                    shareObject = shareDirectory.shareObject;
                    Nfs3Procedure.Status status = shareObject.CheckStatus();
                    if(status != Nfs3Procedure.Status.Ok)
                        throw new InvalidOperationException(String.Format("The root share directory [{0}] has become invalid (status={1})", shareDirectory, status));
                    return Nfs3Procedure.Status.Ok;
                }
            }

            shareObject = null;
            return Nfs3Procedure.Status.ErrorNoSuchFileOrDirectory;
        }
        public Nfs3Procedure.Status TryGetSharedObject(String localPathAndName, String shareName, out ShareObject shareObject)
        {
            if (shareObjectsByLocalPath.TryGetValue(localPathAndName, out shareObject))
            {
                Nfs3Procedure.Status status = shareObject.CheckStatus();
                if (status != Nfs3Procedure.Status.Ok) DisposeShareObject(shareObject);
                return status;
            }
            else
            {
                if(File.Exists(localPathAndName))
                {
                    shareObject = CreateNewShareObject(Nfs3Procedure.FileType.Regular, localPathAndName, shareName);
                    return Nfs3Procedure.Status.Ok;
                }
                else if (Directory.Exists(localPathAndName))
                {
                    shareObject = CreateNewShareObject(Nfs3Procedure.FileType.Directory, localPathAndName, shareName);
                    return Nfs3Procedure.Status.Ok;
                }
                else
                {
                    return Nfs3Procedure.Status.ErrorNoSuchFileOrDirectory;
                }
            }
        }
        public ShareObject TryGetSharedObject(Nfs3Procedure.FileType expectedFileType, String parentDirectory, String localPathAndName)
        {
            switch (expectedFileType)
            {
                case Nfs3Procedure.FileType.Regular:
                    if (!File.Exists(localPathAndName)) return null;
                    break;
                case Nfs3Procedure.FileType.Directory:
                    if (!Directory.Exists(localPathAndName)) return null;
                    break;
                default:
                    return null;
            }

            ShareObject shareObject;
            if (shareObjectsByLocalPath.TryGetValue(localPathAndName, out shareObject))
            {
                if (shareObject.fileType == expectedFileType) return shareObject;
                DisposeShareObject(shareObject);
            }

            String shareName = NfsPath.LocalPathDiff(parentDirectory, localPathAndName);
            if (!NfsPath.IsValidUnixFileName(shareName))
                throw new InvalidOperationException(String.Format("The file you supplied '{0}' is not a valid unix file name", shareName));

            return CreateNewShareObject(expectedFileType, localPathAndName, shareName);
        }
    }
}