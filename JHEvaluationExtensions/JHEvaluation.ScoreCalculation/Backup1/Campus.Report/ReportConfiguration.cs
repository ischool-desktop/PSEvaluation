using System;
using System.Collections.Generic;
using System.Text;
using K12.Data.Configuration;

namespace Campus.Report
{
    public class ReportConfiguration
    {
        private const string ConstTemplateBase64String = "__TemplateBase64String__";
        private const string ConstTemplateType = "__TemplateType__";

        /// <summary>
        /// 報表名稱。
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// 報表樣板。
        /// </summary>
        public ReportTemplate Template { get; set; }
        /// <summary>
        /// ConfigData Instance
        /// </summary>
        private ConfigData ConfigData { get; set; }

        /// <summary>
        /// 傳入報表名稱，建立 ReportConfiguration。
        /// </summary>
        /// <param name="name"></param>
        public ReportConfiguration(string name)
        {
            Name = name;
            ConfigData = K12.Data.School.Configuration[Name];
            LoadTemplate();
        }

        /// <summary>
        /// 判斷指定的名稱是否已存在組態中。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Contains(string name)
        {
            return ConfigData.Contains(name);
        }

        #region Get/Set Methods
        /// <summary>
        /// 取得 String 類型的設定值。
        /// </summary>
        /// <param name="name">索引值</param>
        /// <param name="defaultValue">預設值</param>
        /// <returns></returns>
        public string GetString(string name, string defaultValue)
        {
            if (ConfigData.Contains(name))
                return ConfigData[name];
            else
                return defaultValue;
        }

        /// <summary>
        /// 設定 String 類型的設定值。
        /// </summary>
        /// <param name="name">索引值</param>
        /// <param name="value">設定值</param>
        public void SetString(string name, string value)
        {
            ConfigData[name] = value;
        }

        /// <summary>
        /// 取得 Boolean 類型的設定值。
        /// </summary>
        /// <param name="name">索引值</param>
        /// <param name="defaultValue">預設值</param>
        /// <returns></returns>
        public bool GetBoolean(string name, bool defaultValue)
        {
            return ConfigData.GetBoolean(name, defaultValue);
        }

        /// <summary>
        /// 設定 Boolean 類型的設定值。
        /// </summary>
        /// <param name="name">索引值</param>
        /// <param name="value">設定值</param>
        public void SetBoolean(string name, bool value)
        {
            ConfigData[name] = value.ToString();
        }

        /// <summary>
        /// 取得 Integer 類型的設定值。
        /// </summary>
        /// <param name="name">索引值</param>
        /// <param name="defaultValue">預設值</param>
        /// <returns></returns>
        public int GetInteger(string name, int defaultValue)
        {
            return ConfigData.GetInteger(name, defaultValue);
        }

        /// <summary>
        /// 設定 Integer 類型的設定值。
        /// </summary>
        /// <param name="name">索引值</param>
        /// <param name="value">設定值</param>
        public void SetInteger(string name, int value)
        {
            ConfigData[name] = value.ToString();
        }
        #endregion

        /// <summary>
        /// 讀取報表設定值。
        /// </summary>
        //public void Load()
        //{
        //    LoadTemplate();
        //}

        private void LoadTemplate()
        {
            string base64 = string.Empty;
            TemplateType type = TemplateType.Word;

            if (ConfigData.Contains(ConstTemplateBase64String))
                base64 = ConfigData[ConstTemplateBase64String];
            if (ConfigData.Contains(ConstTemplateType))
            {
                string type_string = ConfigData[ConstTemplateType];
                if (type_string == TemplateType.Excel.ToString())
                    type = TemplateType.Excel;
                else if (type_string == TemplateType.Word.ToString())
                    type = TemplateType.Word;
                else
                    throw new Exception("無效的 TemplateType");
            }

            if (!string.IsNullOrEmpty(base64))
                Template = new ReportTemplate(base64, type);
        }

        private void SaveTemplate()
        {
            string base64 = string.Empty;
            TemplateType type = TemplateType.Word;
            if (Template != null)
            {
                base64 = Template.ToBase64();
                type = Template.Type;
            }
            ConfigData[ConstTemplateBase64String] = base64;
            ConfigData[ConstTemplateType] = type.ToString();
        }

        /// <summary>
        /// 儲存報表設定值。
        /// </summary>
        public void Save()
        {
            SaveTemplate();
            ConfigData.Save();
        }
    }
}
