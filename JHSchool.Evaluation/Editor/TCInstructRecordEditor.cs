using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Editor;
using Framework;
using FISCA.DSAUtil;

namespace JHSchool.Evaluation.Editor
{
    public class TCInstructRecordEditor
    {
        /// <summary>
        /// 取得或設定，指出是否從資料庫刪除這筆資料
        /// </summary>
        public bool Remove { get; set; }
        internal string ID { get; set; }
        internal string RefTeacherID { get; set; }
        internal string RefCourseID { get; set; }
        private bool NoChange { get; set; }

        /// <summary>
        /// 取得或設定教師順序。
        /// </summary>
        public string Sequence { get; internal set; }

        /// <summary>
        /// 取得教師
        /// </summary>
        public TeacherRecord Teacher
        {
            get { return JHSchool.Teacher.Instance[RefTeacherID]; }
        }

        /// <summary>
        /// 取得或設定課程
        /// </summary>
        public CourseRecord Course
        {
            get { return JHSchool.Course.Instance[RefCourseID]; }
        }

        /// <summary>
        /// 取得修改狀態
        /// </summary>
        public EditorStatus EditorStatus
        {
            get
            {
                if (NoChange) return EditorStatus.NoChanged; //如果 NoChange 是 True 就一定是 NoChange。

                if (TCInstructRecord == null)
                {
                    if (!Remove)
                        return EditorStatus.Insert;
                    else
                        return EditorStatus.NoChanged;
                }
                else
                {
                    if (Remove)
                        return EditorStatus.Delete;
                    if (
                        TCInstructRecord.RefCourseID != this.RefCourseID ||
                        TCInstructRecord.RefTeacherID != this.RefTeacherID ||
                        TCInstructRecord.Sequence != this.Sequence
                        )
                    {
                        return EditorStatus.Update;
                    }
                }
                return EditorStatus.NoChanged;
            }
        }

        internal TCInstructRecord TCInstructRecord { get; set; }

        public void Save()
        {
            new TCInstructRecordEditor[] { this }.SaveAllEditors();
        }

        /// <summary>
        /// 建立什麼都不要改變的 Editor。
        /// </summary>
        internal TCInstructRecordEditor()
        {
            NoChange = true;
        }

        /// <summary>
        /// 建立「刪除」的 Editor。
        /// </summary>
        /// <param name="info"></param>
        internal TCInstructRecordEditor(TCInstructRecord info)
        {
            NoChange = false;
            ID = info.ID;
            Remove = true;
        }

        /// <summary>
        /// 建立變更教師的 Editor。
        /// </summary>
        internal TCInstructRecordEditor(TCInstructRecord info, TeacherRecord teacher)
        {
            NoChange = false;
            TCInstructRecord = info;
            ID = info.ID;
            RefCourseID = info.RefCourseID;
            RefTeacherID = teacher.ID; //使用指定的教師。
            Sequence = info.Sequence;
        }

        /// <summary>
        /// 建立新增授課教師的 Editor。
        /// </summary>
        internal TCInstructRecordEditor(CourseRecord course, TeacherRecord teacher, string sequence)
        {
            NoChange = false;
            TCInstructRecord = null;
            ID = string.Empty;
            RefTeacherID = teacher.ID;
            RefCourseID = course.ID;
            Sequence = sequence;
        }
    }

    public static class TCInstructRecordEditor_ExtendMethods
    {
        /// <summary>
        /// 設定課程的第一位教師。
        /// </summary>
        /// <param name="teacher">設定成 Null 即為移除教師。</param>
        public static TCInstructRecordEditor SetFirstTeacher(this CourseRecord course, TeacherRecord teacher)
        {
            return SetTeacher(course, teacher, "1");
        }

        /// <summary>
        /// 設定課程的第二位教師。
        /// </summary>
        /// <param name="teacher">設定成 Null 即為移除教師。</param>
        public static TCInstructRecordEditor SetSecondTeacher(this CourseRecord course, TeacherRecord teacher)
        {
            return SetTeacher(course, teacher, "2");
        }

        /// <summary>
        /// 設定課程的第三位教師。
        /// </summary>
        /// <param name="teacher">設定成 Null 即為移除教師。</param>
        public static TCInstructRecordEditor SetThirdTeacher(this CourseRecord course, TeacherRecord teacher)
        {
            return SetTeacher(course, teacher, "3");
        }

        private static TCInstructRecordEditor SetTeacher(CourseRecord course, TeacherRecord teacher, string sequence)
        {
            TCInstructRecord tcrecord;

            if (sequence == "1")
                tcrecord = course.GetFirstInstruct();
            else if (sequence == "2")
                tcrecord = course.GetSecondInstruct();
            else
                tcrecord = course.GetThirdInstruct();

            if (tcrecord != null && teacher != null)
                return new TCInstructRecordEditor(tcrecord, teacher); //改變教師
            else if (tcrecord != null && teacher == null)
                return new TCInstructRecordEditor(tcrecord); //刪除記錄。
            else if (tcrecord == null && teacher == null)
                return new TCInstructRecordEditor();    //什麼都不作。
            else
                return new TCInstructRecordEditor(course, teacher, sequence); //新增記錄。
        }

        /// <summary>
        /// 儲存所有教授設定。
        /// </summary>
        /// <param name="records"></param>
        public static void SaveAllEditors(this IEnumerable<TCInstructRecordEditor> records)
        {
            MultiThreadWorker<TCInstructRecordEditor> worker = new MultiThreadWorker<TCInstructRecordEditor>();
            worker.MaxThreads = 3;
            worker.PackageSize = 100;
            worker.PackageWorker += delegate(object sender, PackageWorkEventArgs<TCInstructRecordEditor> e)
            {
                DSXmlHelper updateHelper = new DSXmlHelper("Request");
                DSXmlHelper insertHelper = new DSXmlHelper("Request");
                DSXmlHelper deleteHelper = new DSXmlHelper("Request");
                List<string> synclist = new List<string>();
                bool hasUpdate = false;
                bool hasInsert = false;
                bool hasRemove = false;
                foreach (var editor in e.List)
                {
                    if (editor.EditorStatus == EditorStatus.Insert)
                    {
                        #region 新增修課
                        insertHelper.AddElement("Instruct");
                        insertHelper.AddElement("Instruct", "RefTeacherID", editor.RefTeacherID);
                        insertHelper.AddElement("Instruct", "RefCourseID", editor.RefCourseID);
                        insertHelper.AddElement("Instruct", "Sequence", editor.Sequence);
                        hasInsert = true;
                        #endregion
                    }
                    if (editor.EditorStatus == EditorStatus.Update)
                    {
                        #region 修改
                        updateHelper.AddElement("Instruct");
                        updateHelper.AddElement("Instruct", "RefTeacherID", editor.RefTeacherID);
                        updateHelper.AddElement("Instruct", "RefCourseID", editor.RefCourseID);
                        updateHelper.AddElement("Instruct", "Sequence", editor.Sequence);
                        updateHelper.AddElement("Instruct", "Condition");
                        updateHelper.AddElement("Instruct/Condition", "ID", editor.TCInstructRecord.ID);
                        synclist.Add(editor.TCInstructRecord.ID);
                        hasUpdate = true;
                        #endregion
                    }
                    if (editor.EditorStatus == EditorStatus.Delete)
                    {
                        #region 刪除
                        deleteHelper.AddElement("Instruct");
                        deleteHelper.AddElement("Instruct", "ID", editor.TCInstructRecord.ID);
                        synclist.Add(editor.TCInstructRecord.ID);
                        hasRemove = true;
                        #endregion
                    }
                }

                if (hasInsert)
                {
                    DSResponse resp = FISCA.Authentication.DSAServices.CallService("SmartSchool.Course.InsertTCInstruct", new DSRequest(insertHelper.BaseElement));
                    foreach (var item in resp.GetContent().GetElements("NewID"))
                        synclist.Add(item.InnerText);
                }

                if (hasUpdate)
                    FISCA.Authentication.DSAServices.CallService("SmartSchool.Course.UpdateTCInstruct", new DSRequest(updateHelper.BaseElement));

                if (hasRemove)
                    FISCA.Authentication.DSAServices.CallService("SmartSchool.Course.DeleteTCInstruct", new DSRequest(deleteHelper.BaseElement));

                if (synclist.Count > 0)
                    TCInstruct.Instance.SyncDataBackground(synclist);
            };
            List<PackageWorkEventArgs<TCInstructRecordEditor>> packages = worker.Run(records);
            foreach (PackageWorkEventArgs<TCInstructRecordEditor> each in packages)
            {
                if (each.HasException)
                    throw each.Exception;
            }
        }
    }
}
