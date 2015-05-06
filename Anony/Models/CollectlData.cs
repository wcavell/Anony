using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Anony.Controls;
using Newtonsoft.Json;

namespace Anony.Models
{
    public class CollectlData
    {

        public static async Task<List<Bunch>> GetData()
        {
            StorageFile file = null;
            try
            {
                file = await ApplicationData.Current.LocalFolder.GetFileAsync("collect.cfg");
            }
            catch
            {
                return new List<Bunch>();
            }
            if (file == null) return new List<Bunch>();
            var str = await FileIO.ReadTextAsync(file);
            if (string.IsNullOrEmpty(str)) return new List<Bunch>();
            try
            {
                return JsonConvert.DeserializeObject<List<Bunch>>(str);
            }
            catch
            {
                return new List<Bunch>();
            }
        }

        public static async void SaveData(ICollection<Bunch> bunches)
        {
            try
            {
                var file =
                    await
                        ApplicationData.Current.LocalFolder.CreateFileAsync("collect.cfg",
                            CreationCollisionOption.ReplaceExisting);

                var str = JsonConvert.SerializeObject(bunches);
                await FileIO.WriteTextAsync(file, str);
                Toast.Show("操作成功");
            }
            catch
            {
                Toast.ShowError("操作失败");
            }
        }

        public static async void SaveData(Bunch bun)
        {
            var items = await GetData();
            if (items.FirstOrDefault(x => x.Id == bun.Id) == null)
            {
                items.Add(bun);
                SaveData(items);
            }
            else
            {
                Toast.ShowError("已存在");
            }
        }

        public static async void DeleteAll()
        {
            try
            {
                var file =await ApplicationData.Current.LocalFolder.GetFileAsync("collect.cfg");
                await file.DeleteAsync();
            }
            catch { }
        }
    }
}
