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
	public class RSIwithStopAndTarget : Strategy
	{
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name = "RSI Period", Order = 1, GroupName = "Parameters")]
		public int RSIPeriod { get; set; } = 14; // Set a default value of 14
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name = "RSI Smooth", Order = 2, GroupName = "Parameters")]
		public int RSISmooth { get; set; } = 3; // Set a default value of 3

		[NinjaScriptProperty]
    	[Range(1, int.MaxValue)]
    	[Display(Name = "ProfitTarget", Order = 3, GroupName = "Parameters")]
    	public int ProfitTarget { get; set;} = 8;
		
		[NinjaScriptProperty]
    	[Range(1, int.MaxValue)]
    	[Display(Name = "StopLoss", Order = 4, GroupName = "Parameters")]
    	public int StopLoss { get; set;} = 4;
		

		
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				/*
				Description									= @"RSIwithStopAndTarget";
				Name										= "RSIwithStopAndTarget";
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

    			Description                               = @"RSI with a Stop Loss and Profit Target";
    			Name                                     = "RSIwithStopAndTarget";
    			Calculate                                 = Calculate.OnBarClose;
    			EntriesPerDirection                       = 1;
    			EntryHandling                             = EntryHandling.AllEntries;
    			IsExitOnSessionCloseStrategy             = true;
    			ExitOnSessionCloseSeconds                 = 30;
    			IsFillLimitOnTouch                       = false;
    			MaximumBarsLookBack                       = MaximumBarsLookBack.TwoHundredFiftySix;
    			OrderFillResolution                       = OrderFillResolution.Standard;
    			Slippage                                 = 0;
    			StartBehavior                             = StartBehavior.WaitUntilFlat;
    			TimeInForce                               = TimeInForce.Gtc;
    			TraceOrders                               = true;
    			RealtimeErrorHandling                     = RealtimeErrorHandling.StopCancelClose;
    			StopTargetHandling                       = StopTargetHandling.PerEntryExecution;
    			BarsRequiredToTrade                       = 20;
    			// Disable this property for performance gains in Strategy Analyzer optimizations
    			// See the Help Guide for additional information
    			IsInstantiatedOnEachOptimizationIteration = true;
    			//RSIPeriod                                 = 14;
    			//RSISmooth                                 = 3;
    			//ProfitTarget                             = 12;
    			//StopLoss                                 = 6;
			}
  			else if (State == State.DataLoaded)
    		{
    			AddChartIndicator(RSI(RSIPeriod, RSISmooth));
     
    			SetStopLoss(CalculationMode.Ticks, StopLoss);
    			SetProfitTarget(CalculationMode.Ticks, ProfitTarget);
			}
			else if (State == State.Configure)
			{
			}
		}

		protected override void OnBarUpdate()
		{
			//Add your custom strategy logic here.
  			if (CurrentBar < RSIPeriod)
    			return;
 
  			if(CrossAbove(RSI(RSIPeriod, RSISmooth), 20, 1))
    			EnterLong();

			if(CrossBelow(RSI(RSIPeriod, RSISmooth), 20, 1))
    			EnterShort();
		}
	}
}
