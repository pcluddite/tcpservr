/**
 * drdandle
 * The Code Project Open License 1.02
 * http://www.codeproject.com/Articles/16343/Copy-and-Rename-Registry-Keys
 **/

using Microsoft.Win32;

// RenameRegistryKey © Copyright 2006 Active Computing
namespace Tbasic.Components
{
    internal class RegistryUtilities
    {

        internal static object Read(RegistryKey keybase, string keypath, string keyname, object defaultVal)
        {
            using (RegistryKey key = keybase.OpenSubKey(keypath)) {
                return key.GetValue(keyname, defaultVal);
            }
        }

        /// <summary>
        /// Renames a subkey of the passed in registry key since 
        /// the Framework totally forgot to include such a handy feature.
        /// </summary>
        /// <param name="parentKey">The RegistryKey that contains the subkey 
        /// you want to rename (must be writeable)</param>
        /// <param name="subKeyName">The name of the subkey that you want to rename
        /// </param>
        /// <param name="newSubKeyName">The new name of the RegistryKey</param>
        /// <returns>True if succeeds</returns>
        internal static bool RenameSubKey(RegistryKey parentKey,
            string subKeyName, string newSubKeyName)
        {
            CopyKey(parentKey, subKeyName, newSubKeyName);
            parentKey.DeleteSubKeyTree(subKeyName);
            return true;
        }

        /// <summary>
        /// Copy a registry key.  The parentKey must be writeable.
        /// </summary>
        /// <param name="parentKey"></param>
        /// <param name="keyNameToCopy"></param>
        /// <param name="newKeyName"></param>
        /// <returns></returns>
        internal static bool CopyKey(RegistryKey parentKey,
            string keyNameToCopy, string newKeyName)
        {
            //Create new key
            using (RegistryKey destinationKey = parentKey.CreateSubKey(newKeyName)) {

                //Open the sourceKey we are copying from
                using (RegistryKey sourceKey = parentKey.OpenSubKey(keyNameToCopy)) {

                    RecurseCopyKey(sourceKey, destinationKey);

                    return true;
                }
            }
        }

        private static void RecurseCopyKey(RegistryKey sourceKey, RegistryKey destinationKey)
        {
            //copy all the values
            foreach (string valueName in sourceKey.GetValueNames())
            {
                object objValue = sourceKey.GetValue(valueName);
                RegistryValueKind valKind = sourceKey.GetValueKind(valueName);
                destinationKey.SetValue(valueName, objValue, valKind);
            }

            //For Each subKey 
            //Create a new subKey in destinationKey 
            //Call myself 
            foreach (string sourceSubKeyName in sourceKey.GetSubKeyNames())
            {
                RegistryKey sourceSubKey = sourceKey.OpenSubKey(sourceSubKeyName);
                RegistryKey destSubKey = destinationKey.CreateSubKey(sourceSubKeyName);
                RecurseCopyKey(sourceSubKey, destSubKey);
            }
        }
    }
}
