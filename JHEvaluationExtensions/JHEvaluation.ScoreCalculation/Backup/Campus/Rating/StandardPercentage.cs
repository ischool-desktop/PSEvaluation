using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Campus.Rating
{
    /// <summary>
    /// 教育部公佈的標準百分排名算法。
    /// </summary>
    public class StandardPercentage
    {
        /// <summary>
        /// 百分排名空間。
        /// </summary>
        private Queue<Space> Levels = new Queue<Space>();

        private int PreviousPlace { get; set; }
        private int PreviousPercentage { get; set; }

        private int GetCapabilitySum()
        {
            if (Levels.Count <= 0)
                return 0;
            else
                return Levels.Sum(x => x.Capability);
        }

        #region IPercentageAlgorithm 成員

        /// <summary>
        /// 
        /// </summary>
        /// <param name="radix"></param>
        public void Initital(int radix)
        {
            if (radix < 1) throw new ArgumentException("基數必需大於等於 1。");

            PreviousPlace = int.MinValue;
            PreviousPercentage = int.MinValue;
            Levels = new Queue<Space>();
            for (int i = 1; i <= 100; i++)
            {
                int cap = (int)Math.Ceiling((i * radix) / 100m);
                Levels.Enqueue(new Space(i, cap - GetCapabilitySum()));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="place"></param>
        /// <returns></returns>
        public int NextPercentage(int place)
        {
            Space space = GetSpace();
            if (space == null)
                throw new ArgumentException("人數已經超過百分排名空間。");

            space.Capability--; //不管如何要少一人。
            int result = int.MinValue;

            if (place > PreviousPlace)
                result = space.Percentage;
            else
                result = PreviousPercentage;

            PreviousPlace = place;
            PreviousPercentage = result;

            return result;
        }

        private Space GetSpace()
        {
            while (Levels.Count > 0)
            {
                Space space = Levels.Peek();
                if (space.Capability <= 0)
                    Levels.Dequeue();
                else
                    return space;
            }
            return null;
        }

        #endregion

        class Space
        {
            public Space(int percentage, int capability)
            {
                Percentage = percentage;
                Capability = capability;
            }

            public int Percentage { get; private set; }

            public int Capability { get; set; }
        }
    }
}
