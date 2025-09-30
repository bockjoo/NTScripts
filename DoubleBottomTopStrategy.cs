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
	public class DoubleBottomTopStrategy : Strategy
	{
	    // User-configurable parameters
	    [NinjaScriptProperty]
	    [Range(1, int.MaxValue)]
	    [Display(Name = "LookbackBars", Order = 0, GroupName = "Parameters")]
	    public int LookbackBars { get; set; }
	
	    [NinjaScriptProperty]
	    [Range(1, int.MaxValue)]
	    [Display(Name = "PriceToleranceTicks", Order = 1, GroupName = "Parameters")]
	    public int PriceToleranceTicks { get; set; }
	
	    [NinjaScriptProperty]
	    [Range(1, int.MaxValue)]
	    [Display(Name = "NecklineConfirmTicks", Order = 2, GroupName = "Parameters")]
	    public int NecklineConfirmTicks { get; set; }

	    // State variables
	    private double firstBottomPrice;
	    private int firstBottomBar;
	    private double secondBottomPrice;
	    private int secondBottomBar;
	    private double necklinePrice;

	    protected override void OnStateChange()
	    {
	        if (State == State.SetDefaults)
	        {
	            // Set initial strategy defaults
	            Name = "MyDoubleBottomStrategy";
	            Calculate = Calculate.OnBarClose;
	            EntriesPerDirection = 1;
	            //Is  = true;
	            //EntryHandling = EntryHandling.AllScripts;
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
	
	            // Set default parameter values
	            LookbackBars = 40;
	            PriceToleranceTicks = 2;
	            NecklineConfirmTicks = 2;
	        }
	    }

	    protected override void OnBarUpdate()
	    {
	        // Only process historical data on the first bar update
	        if (CurrentBars[0] < LookbackBars + 10)
	        {
	            return;
	        }
	
	        // Check if we are currently in a trade
	        if (Position.MarketPosition != MarketPosition.Flat)
	        {
	            return;
	        }
	
	        //--- Pattern Detection Logic ---
	        DetectDoubleBottom();
	
	        //--- Entry and Exit Logic ---
	        ExecuteTrades();
	    }

	    private void DetectDoubleBottom()
	    {
	        firstBottomPrice = 0;
	        secondBottomPrice = 0;
	        necklinePrice = 0;
	
	        // Find the first bottom
	        for (int i = LookbackBars; i >= 10; i--)
	        {
	            // Use Swing() indicator to find a swing low
	            if (Swing(Low, 5).SwingLow[i] > 0)
	            {
	                firstBottomPrice = Swing(Low, 5).SwingLow[i];
	                firstBottomBar = CurrentBar - i;
	
	                // Look for the second bottom within the defined period
	                for (int j = i - 5; j >= 1; j--)
	                {
	                    if (Swing(Low, 5).SwingLow[j] > 0 && Math.Abs(Swing(Low, 5).SwingLow[j] - firstBottomPrice) <= PriceToleranceTicks * TickSize)
	                    {
	                        secondBottomPrice = Swing(Low, 5).SwingLow[j];
	                        secondBottomBar = CurrentBar - j;
	
	                        // Find the neckline (intermediate peak)
	                        for (int k = secondBottomBar; k > firstBottomBar; k--)
	                        {
	                            if (Swing(High, 5).SwingHigh[k] > 0)
	                            {
	                                necklinePrice = Swing(High, 5).SwingHigh[k];
	                                return; // Pattern found, exit loops
	                            }
	                        }
	                    }
	                }
	            }
	        }
	    }

	    private void ExecuteTrades()
	    {
	        // Entry condition: Check if a pattern was found and the price breaks above the neckline
	        if (necklinePrice > 0 && Close[0] > necklinePrice + NecklineConfirmTicks * TickSize)
	        {
	            // Enter a long position
	            EnterLong();
	        }
	    }
	}
}
