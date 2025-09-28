#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Gui.Tools;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.Indicators;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

namespace NinjaTrader.NinjaScript.Strategies
{
 public class SmoothMARibbonStrategy : Strategy
 {
    private EMA fastEMA;
    private EMA slowEMA;

    [NinjaScriptProperty]
    [Range(1, int.MaxValue)]
    [Display(Name = "Fast EMA Period", Order = 1, GroupName = "Parameters")]
    public int FastPeriod { get; set; }

    [NinjaScriptProperty]
    [Range(1, int.MaxValue)]
    [Display(Name = "Slow EMA Period", Order = 2, GroupName = "Parameters")]
    public int SlowPeriod { get; set; }

    protected override void OnStateChange()
    {
        if (State == State.SetDefaults)
        {
            Description = @"A smooth MA ribbon strategy using EMAs.";
            Name = "SmoothMARibbonStrategy";
            Calculate = Calculate.OnBarClose;
            EntriesPerDirection = 1;
            EntryHandling = EntryHandling.AllEntries;
			/*
            Is(Strategy.IsExitOnSessionCloseStrategy) = false;
            Is(Strategy.IsFillLimitOnTouch) = false;
            Is(Strategy.IsUnmanaged) = false;
			*/
            IsExitOnSessionCloseStrategy = false;
            IsFillLimitOnTouch = false;
            IsUnmanaged = false;
            FastPeriod = 10;
            SlowPeriod = 50;
	        //AddPlot(Color.Goldenrod, "fastEMA");
    	    //AddPlot(Color.LightSalmon, "slowEMA");	
			AddPlot(Brushes.Blue, "fastEMA");
			AddPlot(Brushes.Red, "slowEMA");
        }
        else if (State == State.Configure)
        {
            fastEMA = EMA(FastPeriod);
            slowEMA = EMA(SlowPeriod);

            //Add(fastEMA);
            //Add(slowEMA);
			
        }
    }

    protected override void OnBarUpdate()
    {
        if (CurrentBar < Math.Max(FastPeriod, SlowPeriod))
            return;

        // Long entry condition: Fast EMA crosses above slow EMA
        if (CrossAbove(fastEMA, slowEMA, 1))
        {
            EnterLong();
			//Print("EnterLong");

        }

        // Short entry condition: Fast EMA crosses below slow EMA
        if (CrossBelow(fastEMA, slowEMA, 1))
        {
            EnterShort();
			//Print("EnterShort");
        }

        // Exit conditions
        if (Position.MarketPosition == MarketPosition.Long)
        {
            // Exit if the ribbon starts to compress and reverse
            if (CrossBelow(fastEMA, slowEMA, 1))
            {
                ExitLong();
				//Print("ExitLong");
            }
        }
        else if (Position.MarketPosition == MarketPosition.Short)
        {
            // Exit if the ribbon starts to compress and reverse
            if (CrossAbove(fastEMA, slowEMA, 1))
            {
                ExitShort();
				//Print("ExitShort");

            }
        }
    }
 }
}
