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
using NinjaTrader.NinjaScript.DrawingTools;
using NinjaTrader.NinjaScript.Indicators;

namespace NinjaTrader.NinjaScript.Strategies
{
    public class MomentumScalper : Strategy // Only winning strategy as of Oct 28, 2025
    {
        private EMA ema20;
        private EMA ema50;
        private RSI rsi;
        private MACD macd;

        private double riskRewardRatio = 2.0;
        private int stopLossTicks = 20;
        private int takeProfitTicks = 50;

        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Description = "NQ Futures Scalping Strategy with 1:2 Risk-Reward";
                Name = "MomentumScalper";
                Calculate = Calculate.OnBarClose;
                EntriesPerDirection = 1;
                EntryHandling = EntryHandling.AllEntries;
                IsExitOnSessionCloseStrategy = true;
                ExitOnSessionCloseSeconds = 30;
                IsFillLimitOnTouch = false;
                MaximumBarsLookBack = MaximumBarsLookBack.TwoHundredFiftySix;
                OrderFillResolution = OrderFillResolution.Standard;
                Slippage = 0;
                StartBehavior = StartBehavior.WaitUntilFlat;
                TimeInForce = TimeInForce.Gtc;
                TraceOrders = false;
                RealtimeErrorHandling = RealtimeErrorHandling.StopCancelClose;
                StopTargetHandling = StopTargetHandling.PerEntryExecution;
                BarsRequiredToTrade = 50;
                IsInstantiatedOnEachOptimizationIteration = true;

                // Trading session restrictions (9:30 AM - 4:00 PM ET)
                bool IsSessionStartTimeEnabled = true;
                TimeSpan SessionStartTime = new TimeSpan(9, 30, 0);
                bool IsSessionEndTimeEnabled = true;
                TimeSpan SessionEndTime = new TimeSpan(16, 0, 0);
            }

            else if (State == State.Configure)
            {
                SetProfitTarget(CalculationMode.Ticks, takeProfitTicks);
                SetStopLoss(CalculationMode.Ticks, stopLossTicks);
            }

            else if (State == State.DataLoaded)
            {
                // Initialize indicators
                ema20 = EMA(20);
                ema50 = EMA(50);
                rsi = RSI(14, 1);
                macd = MACD(12, 26, 9);

                // Add indicators to chart
                AddChartIndicator(ema20);
                AddChartIndicator(ema50);
                AddChartIndicator(rsi);
                AddChartIndicator(macd);
            }
        }

        protected override void OnBarUpdate()
        {
            // Ensure we have enough bars for calculation
            if (CurrentBars[0] < BarsRequiredToTrade)
                return;

            // Check session time (9:30 AM - 4:00 PM ET only)
            if (!IsValidTradingTime())
                return;

            // Long entry conditions
            if (Position.MarketPosition == MarketPosition.Flat)
            {
        		// Set the default trail stop to 12 ticks
        		//SetTrailStop(CalculationMode.Ticks, stopLossTicks); // https://www.google.com/search?q=show+me+examples+where+I+can+dynamically+reset+trailstop+in+ninjascript&sca_esv=2fb4accb1dc1e879&sxsrf=AE3TifMoj4aRDXIBzZgKIdsXfxq6ErCjSw%3A1761620383824&ei=nzEAadKOMsq-p84PgoH3uQw&ved=0ahUKEwiS2tOp88WQAxVK38kDHYLAPccQ4dUDCBE&uact=5&oq=show+me+examples+where+I+can+dynamically+reset+trailstop+in+ninjascript&gs_lp=Egxnd3Mtd2l6LXNlcnAiR3Nob3cgbWUgZXhhbXBsZXMgd2hlcmUgSSBjYW4gZHluYW1pY2FsbHkgcmVzZXQgdHJhaWxzdG9wIGluIG5pbmphc2NyaXB0SKfiAVDCDFjJ3wFwBngBkAEAmAHvAaABhjKqAQczMS4yNS4yuAEDyAEA-AEBmAIWoALwEsICChAAGLADGNYEGEfCAggQABiABBiiBMICBRAAGO8FwgIKECEYoAEYwwQYCsICBBAhGAqYAwCIBgGQBgiSBwY0LjE3LjGgB4XKAbIHBjAuMTcuMbgH2RLCBwY0LjE0LjTIByw&sclient=gws-wiz-serp
                // Bullish momentum scalping setup
                if (Close[0] > ema20[0] && 
                    ema20[0] > ema50[0] && 
                    rsi[0] > 50 && rsi[0] < 70 &&
                    macd.Default[0] > macd.Avg[0] &&
                    Volume[0] > Volume[1])
                {
                    EnterLong(1, "Long Entry");
                }

                // Short entry conditions
                else if (Close[0] < ema20[0] && 
                         ema20[0] < ema50[0] && 
                         rsi[0] < 50 && rsi[0] > 30 &&
                         macd.Default[0] < macd.Avg[0] &&
                         Volume[0] > Volume[1])
                {
                    EnterShort(1, "Short Entry");
                }
            }
			// Check if in a long position and if profit target is met
    		if (Position.MarketPosition == MarketPosition.Long ) {
				if ( Position.GetUnrealizedProfitLoss(PerformanceUnit.Ticks, Close[0]) >= takeProfitTicks / 2) // takeProfitTicks / 4 can be 10
    			{
        			// Dynamically reset the trail stop to a tighter 5 ticks to lock in profits
        			//SetTrailStop(CalculationMode.Ticks, takeProfitTicks / 4); // takeProfitTicks / 8 can be 5
    			}
			}

        }

        private bool IsValidTradingTime()
        {
            TimeSpan currentTime = Time[0].TimeOfDay;
            TimeSpan sessionStart = new TimeSpan(9, 30, 0);
            TimeSpan sessionEnd = new TimeSpan(16, 0, 0);

            return currentTime >= sessionStart && currentTime <= sessionEnd;
        }

        protected override void OnOrderUpdate(Order order, double limitPrice, double stopPrice, 
                                            int quantity, int filled, double averageFillPrice, 
                                            OrderState orderState, DateTime time, ErrorCode error, string comment)
        {

		}

        #region Properties
        [NinjaScriptProperty]
        [Range(1, int.MaxValue)]
        [Display(Name="Stop Loss Ticks", Order=1, GroupName="Parameters")]
        public int StopLossTicks
        {
            get { return stopLossTicks; }
            set { stopLossTicks = Math.Max(1, value); }
        }

        [NinjaScriptProperty]
        [Range(1, int.MaxValue)]
        [Display(Name="Take Profit Ticks", Order=2, GroupName="Parameters")]
        public int TakeProfitTicks
        {
            get { return takeProfitTicks; }
            set { takeProfitTicks = Math.Max(1, value); }
        }

        [NinjaScriptProperty]
        [Range(0.1, 10.0)]
        [Display(Name="Risk Reward Ratio", Order=3, GroupName="Parameters")]
        public double RiskRewardRatio
        {
            get { return riskRewardRatio; }
            set { riskRewardRatio = Math.Max(0.1, value); }
        }
        #endregion
    }
}
