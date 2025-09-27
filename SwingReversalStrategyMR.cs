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
	public class SwingReversalStrategyMR : Strategy
	{
		private MACD myMACD;
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Trend reversal strategy using MACD and RSI divergence.. https://www.google.com/search?q=accurate+swing+trading+strategy+in+ninjascript&oq=accurate+swing+trading+strategy+in+ninjascript&gs_lcrp=EgZjaHJvbWUyBggAEEUYOTIHCAEQIRigATIHCAIQIRigATIHCAMQIRigATIHCAQQIRigAdIBCjE0ODkzajBqMTWoAgiwAgHxBfL4SqJYL81u8QXy-EqiWC_Nbg&sourceid=chrome&ie=UTF-8";
				Name										= "SwingReversalStrategyMR";
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
         		//IsExitOnClose = false;
        
        		// Add indicator series
        		AddChartIndicator(MACD(12, 26, 9));
        		AddChartIndicator(RSI(14, 3));
        		AddChartIndicator(ADX(14));

			}
			else if (State == State.Configure)
			{
            	//AddChartIndicator("MACD");

            	// To access the MACD's values for strategy logic, you would do this:
            	//myMACD = MACD(12, 26, 9);
			}
			//else if (State == State.DataLoaded)
			//{
				//AddChartIndicator(VOL());
				//AddChartIndicator(MACD(12, 26, 9));
			//}

		}

		protected override void OnBarUpdate()
		{
			//Add your custom strategy logic here.
    		if (CurrentBars[0] < BarsRequiredToTrade)
        		return;

    		// Condition 1: Bullish MACD Crossover
    		bool bullishMacdCrossover = MACD(12, 26, 9)[0] > 0 && MACD(12, 26, 9)[1] <= 0;
			//bool bullishMacdCrossover = myMACD.Hist[0] > 0 && myMACD.Hist[1] <= 0;


    		// Condition 2: Bullish RSI Divergence (simplified for example)
    		bool bullishRsiDivergence = Low[0] < Low[1] && RSI(14,3)[0] > RSI(14,3)[1];

    		if (bullishMacdCrossover && bullishRsiDivergence)
    		{
       			 // Enter a long position with a default quantity
        		EnterLong();
    		}
			if (Position.MarketPosition == MarketPosition.Long)
    		{
        		// Set a stop-loss below the recent swing low
        		//double stopPrice = Swing(3, false, 0, 15).SwingLow[1] - (0.5 * ATR(14)[0]);
				// https://ninjatrader.com/support/helpguides/nt8/NT%20HelpGuide%20English.html?relative_strength_index_rsi.htm
        		double stopPrice = Swing(3).SwingLowBar(1,1,10) - (0.5 * ATR(14)[0]);
				//ExitLong()
				//ExitLong(int quantity)
				//ExitLong(string fromEntrySignal)
				//ExitLong(string signalName, string fromEntrySignal)
				//ExitLong(int quantity, string signalName, string fromEntrySignal)
        		//ExitLong("Exit on Low", "Stop Loss", stopPrice, Position.Quantity);
                
                // Set the stop-loss order based on the calculated price.
                SetStopLoss(CalculationMode.Ticks, stopPrice);

    		}
		}
	}
}
