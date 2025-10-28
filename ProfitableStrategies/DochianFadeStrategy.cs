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
	public class DonchianFadeStrategy : Strategy
	{
	    private DonchianChannel _donchianChannel;
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"A fade strategy that takes advantage of this behavior by entering short trades at the upper boundary of the channel and long trades at the lower boundary in ninjascript";
				Name										= "DonchianFadeStrategy";
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
            	// Define your strategy parameters here
            	ChannelPeriod = 20;
            	// Set your profit target and stop loss (in ticks)
            	ProfitTargetTicks = 50;
            	StopLossTicks = 20;
			}
			else if (State == State.Configure)
			{
	            _donchianChannel = DonchianChannel(ChannelPeriod);
			}
		}

		protected override void OnBarUpdate()
		{
			//Add your custom strategy logic here.
        	if (CurrentBar < ChannelPeriod) return; // Wait for enough bars to form the channel

        	// Exit conditions
        	if (Position.MarketPosition != MarketPosition.Flat)
        	{
            	// Exit all positions if the channel breaks in the original direction
            	if (Position.MarketPosition == MarketPosition.Long && Close[0] > _donchianChannel.Upper[0])
            	{
                	ExitLong();
                	return;
            	}
            	if (Position.MarketPosition == MarketPosition.Short && Close[0] < _donchianChannel.Lower[0])
            	{
                	ExitShort();
                	return;
            	}
        	}

        	// Fading the upper boundary with a short entry
        	// Enter a short position if the price touches the upper band of the channel,
        	// suggesting a reversal or a fade from resistance.
        	if (Close[0] >= _donchianChannel.Upper[0] && Position.MarketPosition == MarketPosition.Flat)
        	{
            	EnterShort(1, "ShortEntry");
            	// Set your pre-defined profit target and stop loss on entry
            	SetProfitTarget(CalculationMode.Ticks, ProfitTargetTicks);
            	SetStopLoss(CalculationMode.Ticks, StopLossTicks);
        	}

        	// Fading the lower boundary with a long entry
        	// Enter a long position if the price touches the lower band of the channel,
        	// suggesting a bounce or a fade from support.
        	if (Close[0] <= _donchianChannel.Lower[0] && Position.MarketPosition == MarketPosition.Flat)
        	{
            	EnterLong(1, "LongEntry");
            	// Set your pre-defined profit target and stop loss on entry
            	SetProfitTarget(CalculationMode.Ticks, ProfitTargetTicks);
            	SetStopLoss(CalculationMode.Ticks, StopLossTicks);
        	}
		}
		
    	#region Properties
    	[NinjaScriptProperty]
    	[Range(1, int.MaxValue)]
    	[Display(Name="ChannelPeriod", Order=1, GroupName="Parameters")]
    	public int ChannelPeriod
    	{ get; set; }

    	[NinjaScriptProperty]
    	[Range(1, int.MaxValue)]
    	[Display(Name="ProfitTargetTicks", Order=2, GroupName="Parameters")]
    	public int ProfitTargetTicks
    	{ get; set; }

    	[NinjaScriptProperty]
    	[Range(1, int.MaxValue)]
    	[Display(Name="StopLossTicks", Order=3, GroupName="Parameters")]
    	public int StopLossTicks
    	{ get; set; }
		#endregion

	}
}
