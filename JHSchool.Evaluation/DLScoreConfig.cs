using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.DSAUtil;
using Framework;
using System.Xml;

namespace JHSchool.Evaluation
{
    public class DLScoreConfig
    {
        /// <summary>
        /// Singleton Pattern
        /// </summary>
        private static DLScoreConfig config;

        private List<DLScoreConfigCategory> categories = new List<DLScoreConfigCategory>();

        private DLScoreConfig()
        {
        }

        private DLScoreConfig(DSXmlHelper helper)
        {
            foreach(XmlElement elm in helper.GetElements("Category"))
            {
                this.categories.Add(new DLScoreConfigCategory(elm));
            }
        }

        /// <summary>
        /// 取得日常生活表現的組態設定值。
        /// Singleton Pattern
        /// </summary>
        /// <returns></returns>
        public static DLScoreConfig GetConfig()
        {
            if (config != null)
                return config;

            DSResponse dsrsp = FISCA.Authentication.DSAServices.CallService("SmartSchool.Config.GetDLScoreConfig", new DSRequest());
            DSXmlHelper content = dsrsp.GetContent();
            if (content == null)
                return null;
            else
                return new DLScoreConfig(dsrsp.GetContent());
        }

        public List<DLScoreConfigCategory> Categories { get { return this.categories; } }
    }
    public class DLScoreConfigCategory
    {
        private List<KeyValuePair<string, string>> items = new List<KeyValuePair<string, string>>();

        public DLScoreConfigCategory(XmlElement elmCategory)
        {
            this.Name = elmCategory.GetAttribute("@Name");
            foreach (XmlElement elmItem in elmCategory.SelectNodes("Item"))
            {
                string key = elmItem.GetAttribute("name");
                string val = elmItem.GetAttribute("index");
                HasIndex = HasIndex || (val != "");
                items.Add(new KeyValuePair<string,string>(key, val));
            }
        }

        /// <summary>
        /// 對應到 <Category name="日常行為表現"> 的 Name 屬性。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 項目的集合 <Item name="愛整潔" index="" />
        /// </summary>
        public List<KeyValuePair<string, string>> Items
        {
            get { return this.items; }
        }

        /// <summary>
        /// 此 DLScoreConfigCategory 物件的項目 是否有指標內容。
        /// </summary>
        public bool HasIndex { get; set; }

    }


    /* The XML format is following :
     * 
     *   <DLScoreConfig>
	        <Category name="日常行為表現">
		        <Item name="愛整潔" index=""  displayOrder="1" />
		        <Item name="有禮貌" index=""  displayOrder="2"  />
		        <Item name="守秩序" index=""  displayOrder="3"  />
		        <Item name="責任心" index=""  displayOrder="4"  />
		        <Item name="公德心" index=""  displayOrder="5"  />
		        <Item name="友愛關懷" index=""   displayOrder="6" />
		        <Item name="團隊合作" index=""   displayOrder="7" />
            </Category>
	        <Category name="團體活動表現">
		        <Item name="社團活動"   displayOrder="1" />
		        <Item name="學校活動"   displayOrder="2" />
		        <Item name="自治活動"   displayOrder="3" />
		        <Item name="班級活動"   displayOrder="4" />
            </Category>
	        <Category name="公共服務表現">
		        <Item name="校內服務"    displayOrder="1" />
		        <Item name="社區服務"    displayOrder="2" />
            </Category>	
	        <Category name="校內外時特殊表現">
		        <Item name="校內特殊表現"    displayOrder="1" />
		        <Item name="校外特殊表現"    displayOrder="2" />
            </Category>				
	        <Category name="日常生活表現具體建議">		
                <Item name=""    displayOrder="" />
            </Category>						
        </DLScoreConfig>
     * 
     * */
}
