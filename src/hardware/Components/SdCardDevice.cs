﻿using System;

using GHIElectronics.TinyCLR.IO;
using GHIElectronics.TinyCLR.Devices.Storage;

namespace Bytewizer.TinyCLR.Hardware.Components
{
    class SdCardDevice : DisposableObject, IStorageDevice
    {
        private readonly StorageController _storageController;

        public static SdCardDevice Initialize(string name)
        {
            var device = new SdCardDevice(name);

            try
            {
                device.Mount();
            }
            catch
            {
                throw new InvalidOperationException("The micro SD card could not be mounted.");
            }
            return device;
        }

        public SdCardDevice(string name)
        {
            _storageController = StorageController.FromName(name);
        }

        protected override void Dispose(bool disposing)
        {
            // Only managed resources to dispose
            if (!disposing)
                return;

            // Dispose owned objects
            _storageController?.Dispose();

        }

        public void Mount()
        {
            FileSystem.Mount(_storageController.Hdc);
        }

        public void Unmount()
        {
            FileSystem.Unmount(_storageController.Hdc);
        }
    }
}