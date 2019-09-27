using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonTestApplication.DataTypes
{
	class Candle
	{
		public DateTime time { get; set; }
		public double openBid { get; set; }
		public double openAsk { get; set; }
		public double highBid { get; set; }
		public double highAsk { get; set; }
		public double lowBid { get; set; }
		public double lowAsk { get; set; }
		public double closeBid { get; set; }
		public double closeAsk { get; set; }
		public int volume { get; set; }
		public bool complete { get; set; }
	}
}
