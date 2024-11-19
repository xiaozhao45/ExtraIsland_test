using System.Diagnostics;
using System;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using Timer = System.Threading.Timer;

namespace ExtraIsland.StutterEngine;

public class Animator {

    Animator(TimeSpan interval) {
        _ticker = new System.Timers.Timer(interval);
        _interval = interval;
    }

    readonly TimeSpan _interval;
    readonly System.Timers.Timer _ticker;
    
    public void StartTranslateTransform(TranslateTransform translate, TranslateTransformModifier modifier, TimeSpan duration) {
        _ticker.AutoReset = true;
        int tick = 0;
        _ticker.Elapsed += (o,e) => {
            modifier(translate, tick, _interval, duration);
            if (tick * _interval.Milliseconds >= duration.Milliseconds) {
                _ticker.Stop();
            }
            tick ++;
        };
        _ticker.Start();
    }
    
    public delegate void TranslateTransformModifier(TranslateTransform translate, int tick , TimeSpan interval,TimeSpan duration);
}