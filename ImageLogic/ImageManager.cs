using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace ImageLogic
{
    public class ImageManager
    {
        // Get Image File using File Picker
        // Saves the Image data as Bytes and returns ImagePath
        // Okay if returns null!
        public async Task<string> FileBrowserAsync()
        {

            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file == null) return default;
            string newFilePath = $@"{Windows.Storage.ApplicationData.Current.LocalFolder.Path}/{file.Name}";
            byte[] storageFileContent = await GetImageByteAsync(file);

            File.WriteAllBytes(newFilePath, storageFileContent);

            return newFilePath;
        }


        //To be used with FileBrowserAsync method:
        private async Task<byte[]> GetImageByteAsync(StorageFile file) // Get the byte[] data from ImageFile
        {

            using (var inputStream = await file.OpenSequentialReadAsync())
            {
                var readStream = inputStream.AsStreamForRead();

                var byteArray = new byte[readStream.Length];
                await readStream.ReadAsync(byteArray, 0, byteArray.Length);

                return byteArray;
            }
        }
    }
}
