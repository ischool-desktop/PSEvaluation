using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JHSchool.Evaluation.CourseExtendControls
{
    internal class NavViewNode
    {

        public string Name { get; set; }

        private DevComponents.AdvTree.Node mInstanceNode = new DevComponents.AdvTree.Node(); 
        private Dictionary<string, NavViewNode> mNodes=new Dictionary<string,NavViewNode>();
        private List<string> mPrimaryKeys;
        private static Dictionary<DevComponents.AdvTree.Node,List<string>> mNodePrimaryKeys=null;
        private bool mIsRoot=false;

        public NavViewNode()
        {
            mPrimaryKeys = new List<string>(); 
        }

        public bool IsRoot
        {
            get
            {
                return mIsRoot;
            }
            set
            {
                mIsRoot = value;
            }
        }

        public Dictionary<string, NavViewNode> Nodes
        {
            get
            {
                return mNodes;
            }
        }

        public static Dictionary<DevComponents.AdvTree.Node, List<string>> NodePrimaryKeys
        {
            get
            {
                if (mNodePrimaryKeys == null)
                    mNodePrimaryKeys=new Dictionary<DevComponents.AdvTree.Node,List<string>>();
                return mNodePrimaryKeys;
            }
        }


        public void UpdateInstance(bool IsNoRoot)
        {
            //有子節點的情況
            if (mNodes.Count > 0)
            {
                PrimaryKeys.Clear();
                mInstanceNode.Nodes.Clear();

                foreach (NavViewNode Node in mNodes.Values)
                {
                    Node.UpdateInstance(false);

                    if (!IsNoRoot)
                    {
                        //新增子節點的PrimaryKey
                        foreach (string Key in Node.PrimaryKeys)
                        {
                            if (!PrimaryKeys.Contains(Key))
                                PrimaryKeys.Add(Key);
                        }

                        //新增子節點的Node
                        mInstanceNode.Nodes.Add(Node.InstanceNode);
                    }
                }

                if (!IsNoRoot)
                {
                    //設定節點的名稱
                    mInstanceNode.Text = Name + "(" + PrimaryKeys.Count + ")";

                    //將PrimaryKes加入到變數內
                    NavViewNode.NodePrimaryKeys.Add(mInstanceNode, PrimaryKeys);
                }
            }
            else //無子節點的情況
            {
                mInstanceNode.Text = Name + "(" + PrimaryKeys.Count + ")";

                NavViewNode.NodePrimaryKeys.Add(mInstanceNode, PrimaryKeys); 
            }
        }

        public List<string> PrimaryKeys
        {
            get
            {
                return mPrimaryKeys;
            }
            set
            {
                mPrimaryKeys = value;
            }
        }

        public NavViewNode SmartAdd(string key)
        {
            if (!mNodes.ContainsKey(key))
            {
                NavViewNode Node = new NavViewNode();
                Node.Name = key;
                mNodes.Add(key, Node);
                return Node;
            }
            else
                return mNodes[key];
        }

        public NavViewNode this[string key] 
        { 
            get 
            { 
                return SmartAdd(key); 
            }
        }


        public DevComponents.AdvTree.Node InstanceNode
        {
            get
            {
                return mInstanceNode;
            }
        }
   }
}