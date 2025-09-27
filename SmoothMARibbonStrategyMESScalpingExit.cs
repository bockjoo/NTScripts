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

//This namespace holds Strategies in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Strategies
{
	public class SmoothMARibbonStrategyMESScalpingExit : Strategy
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
				/*
				Description									= @"Smooth MA Ribbon Strategy with MES Scalping Exist";
				Name										= "SmoothMARibbonStrategyMESScalpingExit";
				Calculate									= Calculate.OnBarClose;
				EntriesPerDirection							= 1;
				EntryHandling								= EntryHandling.AllEntries;
				IsExitOnSessionCloseStrategy				= true;
				ExitOnSessionCloseSeconds					= 30;
				IsFillLimitOnTouch							= false;
				MaximumBarsLookBack							= MaximumBarsLookBack.TwoHundredFiftySix;
				OrderFillResolution							= OrderFillResolution.Standard;
				Slippage									= 0;
				StartBehavior								= StartBehavior.WaitUntilFlat;
				TimeInForce									= TimeInForce.Gtc;
				TraceOrders									= false;
				RealtimeErrorHandling						= RealtimeErrorHandling.StopCancelClose;
				StopTargetHandling							= StopTargetHandling.PerEntryExecution;
				BarsRequiredToTrade							= 20;
				// Disable this property for performance gains in Strategy Analyzer optimizations
				// See the Help Guide for additional information
				IsInstantiatedOnEachOptimizationIteration	= true;
				*/
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
			}
		}

		protected override void OnBarUpdate()
		{
			//Add your custom strategy logic here.
        	if (CurrentBar < Math.Max(FastPeriod, SlowPeriod))
            	return;

        	// Long entry condition: Fast EMA crosses above slow EMA
       	 	if (CrossAbove(fastEMA, slowEMA, 1))
        	{
            	//EnterLong();
				Print("Long");
    			// Check if a position is not already open
    			if (Position.MarketPosition == MarketPosition.Flat)
    			{
      				// Call the ATM strategy to manage the trade after entry
      				//does not compile AtmStrategyCreate(OrderAction.Buy, OrderType.Market, 0, 0, "MES_Scalper_Exit", "MyLongEntry");
    			}
        	}

        	// Short entry condition: Fast EMA crosses below slow EMA
        	if (CrossBelow(fastEMA, slowEMA, 1))
        	{
            	//EnterShort();
				Print("Short");
        	}

        	// Exit conditions
        	if (Position.MarketPosition == MarketPosition.Long)
        	{
            	// Exit if the ribbon starts to compress and reverse
            	if (CrossBelow(fastEMA, slowEMA, 1))
            	{
                	//ExitLong();
					Print("ExitLong");
            	}
        	}
        	else if (Position.MarketPosition == MarketPosition.Short)
        	{
            	// Exit if the ribbon starts to compress and reverse
            	if (CrossAbove(fastEMA, slowEMA, 1))
            	{
                	//ExitShort();
					Print("ExitShort");
            	}	
        	}
		}
	}
}
