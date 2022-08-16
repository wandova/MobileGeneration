using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MobileGeneration
{
    public class MainService
    {
        public List<CityCode> CityCodes { get; private set; } = new List<CityCode>();
        public List<ProvinceCity> ProvinceCitys { get; private set; } = new List<ProvinceCity>();
        public List<IspPrefix> IspPrefixs { get; private set; } = new List<IspPrefix>();

        public MainService()
        {
            LoadData();
        }

        public void LoadData()
        {
            if (!File.Exists("data\\city_code.txt")
                || !File.Exists("data\\province_city.txt")
                || !File.Exists("data\\isp_prefix.txt"))
            {
                return;
            }
            var cityCodes = File.ReadAllLines("data\\city_code.txt");
            foreach (var item in cityCodes)
            {
                var items = item.Split(',');
                if (items.Length == 2)
                {
                    var code = items[1];
                    CityCodes.Add(new CityCode
                    {
                        City = items[0],
                        Code = code,
                        Prefix = code.Substring(0, 3)
                    });
                }
            }
            var provinceCitys = File.ReadAllLines("data\\province_city.txt");
            foreach (var item in provinceCitys)
            {
                var items = item.Split(',');
                if (items.Length == 2)
                {
                    ProvinceCitys.Add(new ProvinceCity
                    {
                        Province = items[0],
                        City = items[1]
                    });
                }
            }
            var ispPrefixs = File.ReadAllLines("data\\isp_prefix.txt");
            foreach (var item in ispPrefixs)
            {
                var items = item.Split(',');
                if (items.Length == 2)
                {
                    IspPrefixs.Add(new IspPrefix
                    {
                        Isp = items[0],
                        Prefix = items[1]
                    });
                }
            }
        }

        public TreeNode[] GetIspPrefixTreeNodes()
        {
            var treeNodes = new List<TreeNode>();
            var groups = IspPrefixs.GroupBy(x => x.Isp);
            foreach (var group in groups)
            {
                var treeNode = new TreeNode { Text = group.Key, Name = group.Key };
                foreach (var ispPrefix in group)
                {
                    treeNode.Nodes.Add(new TreeNode { Text = ispPrefix.Prefix, Name = ispPrefix.Prefix });
                }
                treeNodes.Add(treeNode);
            }
            return treeNodes.ToArray();
        }

        public TreeNode[] GetProvinceCityTreeNodes()
        {
            var treeNodes = new List<TreeNode>();
            var groups = ProvinceCitys.GroupBy(x => x.Province);
            foreach (var group in groups)
            {
                var treeNode = new TreeNode { Text = group.Key, Name = group.Key };
                foreach (var provinceCity in group)
                {
                    treeNode.Nodes.Add(new TreeNode { Text = provinceCity.City, Name = provinceCity.City });
                }
                treeNodes.Add(treeNode);
            }
            return treeNodes.ToArray();
        }
    }
}