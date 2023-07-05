using System;
using System.Collections.Generic;

namespace USDT.Core.Table
{
    [Config]
    public partial class ProjectLutCategory : ConfigTable<ProjectLut>
    {
        public static ProjectLutCategory Instance
        {
            get; set;
        }

        public ProjectLutCategory()
        {
            Instance = this;
        }
    }

    public partial class ProjectLut : IConfig
    {
		public int Id { get; set; }
		public string name { get; set; }
		public float shopPrice { get; set; }
		public float meituanPrice { get; set; }
		public float duration { get; set; }
    }
}