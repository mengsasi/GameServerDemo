using System.Collections.Generic;
using UnityEngine;

namespace Core {

    public interface ITicker {
        void Update();
    }

    public class TickerManager : Singleton<TickerManager> {

        private List<ITicker> tickers = new List<ITicker>();
        private bool isUpdate = false;

        private List<ITicker> secondTickers = new List<ITicker>();
        private bool isSecondUpdate = false;
        private float timer = 0f;

        public void Init() {
            timer = 0;
            tickers = new List<ITicker>();
            secondTickers = new List<ITicker>();
        }

        void Update() {
            if( isUpdate ) {
                Refresh( tickers );
            }
            if( isSecondUpdate ) {
                timer += Time.deltaTime;
                if( timer >= 1 ) {
                    timer = 0;
                    Refresh( secondTickers );
                }
            }
        }

        void Refresh( List<ITicker> list ) {
            for( int i = 0; i < list.Count; i++ ) {
                var t = list[i];
                if( t != null ) {
                    t.Update();
                }
                else {
                    list.Remove( t );
                    i--;
                }
            }
        }

        void Register( ITicker ticker ) {
            if( !tickers.Contains( ticker ) ) {
                tickers.Add( ticker );
            }
            isUpdate = true;
        }

        void UnRegister( ITicker ticker ) {
            if( tickers.Contains( ticker ) ) {
                tickers.Remove( ticker );
            }
            if( tickers.Count <= 0 ) {
                isUpdate = false;
            }
        }

        void RegisterSecond( ITicker ticker ) {
            if( !secondTickers.Contains( ticker ) ) {
                secondTickers.Add( ticker );
            }
            isSecondUpdate = true;
        }

        void UnRegisterSecond( ITicker ticker ) {
            if( secondTickers.Contains( ticker ) ) {
                secondTickers.Remove( ticker );
            }
            if( secondTickers.Count <= 0 ) {
                isSecondUpdate = false;
                timer = 0;
            }
        }

        public static void RegisterTicker( ITicker ticker ) {
            TickerManager.Instance.Register( ticker );
        }

        public static void UnRegisterTicker( ITicker ticker ) {
            TickerManager.Instance.UnRegister( ticker );
        }

        public static void RegisterSecondTicker( ITicker ticker ) {
            TickerManager.Instance.RegisterSecond( ticker );
        }

        public static void UnRegisterSecondTicker( ITicker ticker ) {
            TickerManager.Instance.UnRegisterSecond( ticker );
        }

    }

}