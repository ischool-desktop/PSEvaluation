﻿//------------------------------------------------------------------------------
// <auto-generated>
//     這段程式碼是由工具產生的。
//     執行階段版本:4.0.30319.269
//
//     對這個檔案所做的變更可能會造成錯誤的行為，而且如果重新產生程式碼，
//     變更將會遺失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace JHSchool.Evaluation.StuAdminExtendControls.Ribbon.ActivityRecord {
    using System;
    
    
    /// <summary>
    ///   用於查詢當地語系化字串等的強型別資源類別
    /// </summary>
    // 這個類別是自動產生的，是利用 StronglyTypedResourceBuilder
    // 類別透過 ResGen 或 Visual Studio 這類工具。
    // 若要加入或移除成員，請編輯您的 .ResX 檔，然後重新執行 ResGen
    // (利用 /str 選項)，或重建您的 VS 專案。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resource1 {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resource1() {
        }
        
        /// <summary>
        ///   傳回這個類別使用的快取的 ResourceManager 執行個體。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("JHSchool.Evaluation.StuAdminExtendControls.Ribbon.ActivityRecord.Resource1", typeof(Resource1).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   覆寫目前執行緒的 CurrentUICulture 屬性，對象是所有
        ///   使用這個強型別資源類別的資源查閱。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   查詢類似 &lt;ImportRequest&gt;
        ///	&lt;Table Name=&quot;活動表現紀錄&quot;&gt;
        ///		&lt;Field DataType=&quot;String&quot; Indexed=&quot;false&quot; Name=&quot;單位&quot; /&gt;
        ///		&lt;Field DataType=&quot;String&quot; Indexed=&quot;false&quot; Name=&quot;學年度&quot; /&gt;
        ///		&lt;Field DataType=&quot;String&quot; Indexed=&quot;false&quot; Name=&quot;學期&quot; /&gt;
        ///		&lt;Field DataType=&quot;Number&quot; Indexed=&quot;true&quot; Name=&quot;學生編號&quot; /&gt;
        ///		&lt;Field DataType=&quot;String&quot; Indexed=&quot;false&quot; Name=&quot;細項&quot; /&gt;
        ///		&lt;Field DataType=&quot;String&quot; Indexed=&quot;false&quot; Name=&quot;類別&quot; /&gt;
        ///	&lt;/Table&gt;
        ///	&lt;Table Name=&quot;活動表現紀錄對照表&quot;&gt;
        ///		&lt;Field DataType=&quot;String&quot; Indexed=&quot;false&quot; Name=&quot;細項&quot; /&gt;
        ///		&lt;Field DataType=&quot;String&quot; Index [字串的其餘部分已遭截斷]&quot;; 的當地語系化字串。
        /// </summary>
        internal static string ImportTables {
            get {
                return ResourceManager.GetString("ImportTables", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查詢類似 &lt;Package Name=&quot;活動表現&quot;&gt;
        ///	&lt;Service Enabled=&quot;true&quot; Name=&quot;取得活動表現紀錄對照表&quot;&gt;
        ///		&lt;Definition Type=&quot;udt&quot;&gt;
        ///			&lt;UDTConfig&gt;UDT&lt;/UDTConfig&gt;
        ///			&lt;Action&gt;Select&lt;/Action&gt;
        ///			&lt;TableName&gt;活動表現紀錄對照表&lt;/TableName&gt;
        ///			&lt;ResponseRecordElement&gt;對照表/項目&lt;/ResponseRecordElement&gt;
        ///			&lt;FieldList Source=&quot;Field&quot;&gt;
        ///				&lt;Field Mandatory=&quot;True&quot; OutputType=&quot;attribute&quot; Source=&quot;類別&quot; Target=&quot;類別&quot; /&gt;
        ///				&lt;Field Mandatory=&quot;True&quot; OutputType=&quot;attribute&quot; Source=&quot;細項&quot; Target=&quot;細項&quot; /&gt;
        ///				&lt;Field Mandatory=&quot;True&quot; OutputType=&quot;attribute&quot; Source=&quot;數量&quot; Target=&quot;數量 [字串的其餘部分已遭截斷]&quot;; 的當地語系化字串。
        /// </summary>
        internal static string Package {
            get {
                return ResourceManager.GetString("Package", resourceCulture);
            }
        }
    }
}
