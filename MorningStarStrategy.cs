#region Using declarations
using System;
using NinjaTrader.Cbi;
using NinjaTrader.Core;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.NinjaScript.Indicators;
using NinjaTrader.NinjaScript.Strategies;
//using NinjaTrader.NinjaScript.DrawingTools;
//using NinjaTrader.Custom.Indicators;
#endregion

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.NinjaScript.Strategies
{
    public class MorningStarStrategy : Strategy
    {
        private CandlestickPattern morningStarPattern;
		private string atmStrategyId;
		private string atmStrategyOrderId;
		private bool   isAtmStrategyCreated = false;
 
        protected override void OnStateChange()
        {
			
            if (State == State.SetDefaults)
            {
                Description = @"Trades the Morning Star candlestick pattern.";
                Name = "MorningStarStrategy";
                Calculate = Calculate.OnBarClose; // Only evaluate logic on bar close
                EntriesPerDirection = 1;
                IsExitOnSessionCloseStrategy = true;
                IsFillLimitOnTouch = false;
                IsUnmanaged = false;
                BarsRequiredToTrade = 20; // Need at least 20 bars of data
            }
            else if (State == State.Configure)
            {
                // Instantiate the built-in CandleStickPattern indicator
                morningStarPattern = CandlestickPattern(ChartPattern.MorningStar, 0);
            }
        }

        protected override void OnBarUpdate()
        {
			//if (State < State.Realtime)
      		//	return;
            // Only proceed if the strategy is active and there are no open positions
            if (CurrentBar < BarsRequiredToTrade || Position.MarketPosition != MarketPosition.Flat)
            {
                return;
            }
            
            // Check for the Morning Star pattern on the previous bar (index 1)
            // The CandleStickPattern indicator returns 1 when the pattern is found
            if (morningStarPattern[1] == 1)
			//if (CandlestickPattern(ChartPattern.MorningStar, 0)[1] == 1)
			//if (CandlestickPattern(ChartPattern.BullishEngulfing, 4)[0] == 1)	
            {
				atmStrategyId = GetAtmStrategyUniqueId();
      			atmStrategyOrderId = GetAtmStrategyUniqueId();
 				AtmStrategyCreate(OrderAction.Buy, OrderType.Market, 0, 0, TimeInForce.Day,
          			atmStrategyOrderId, "MorningStarATM", atmStrategyId, (atmCallbackErrorCode, atmCallbackId) => {
 
			          	// checks that the call back is returned for the current atmStrategyId stored
          				if (atmCallbackId == atmStrategyId)
          				{
              				// check the atm call back for any error codes
              				if (atmCallbackErrorCode == Cbi.ErrorCode.NoError)
              				{
                  				// if no error, set private bool to true to indicate the atm strategy is created
                	  			isAtmStrategyCreated = true;
              				}
          				}
      			});
                // Enter a long position on the open of the current bar
                //EnterLong();
                
                // Define stop-loss and take-profit targets using an ATM Strategy
                // This ensures automatic risk management after entry
                //AtmStrategyCreate(OrderAction.Buy, OrderType.Market, 0, false, null, "MorningStarATM");
            }
			if(isAtmStrategyCreated)
  			{
      			// atm logic
  			}
	  		else if(!isAtmStrategyCreated)
  			{
      			// custom handling for a failed atm Strategy
  			}
        }
    }
}
