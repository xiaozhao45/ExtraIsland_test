using System.Windows.Controls;
using System.Windows.Media;

namespace ExtraIsland.StutterEngine;

public class LegacyAnimator {
    public LegacyAnimator() {
        LoadCache();
    }
    
    Dictionary<int,double>? _tripleEaseCache;
    static double TripleEase(double tick,double scale = 1,double multiplier = 1) {
        return multiplier * (double.Pow(tick * scale,3));
    }
    public void LoadCache() {
        try {
            _tripleEaseCache = new Dictionary<int,double> { { 0,0.0 } };
            for (int x = 1; x <= 40; x++) {
                _tripleEaseCache.Add(x,40 * TripleEase(x / 40.0));
            }
            for (int x = 1; x <= 40; x++) {
                _tripleEaseCache.Add(-x,-40 * TripleEase(1 - x / 40.0));
            }
        }
        catch {
            // ignored
        }
    }
    
    public void SwapChange(Label target,TranslateTransform targetTransform, string newContent) {
        if (_tripleEaseCache == null) { LoadCache(); }
        for (int  x = 0;  x <= 40;  x++) {
            targetTransform.Y = _tripleEaseCache![x];
            target.Opacity = (40 - x) / 40.0;
            Thread.Sleep(1);
        }
        target.Content = newContent;
        for (int  x = 0;  x <= 40;  x++) {
            targetTransform.Y = _tripleEaseCache![-x];
            target.Opacity =1 - (40 - x) / 40.0;
            Thread.Sleep(1);
        }
    }
    
    public void OpacityEmphasize(Border target,int holdTime = 3000) {
        for (int x = 0; x <= 40; x++) {
            target.Opacity = (1 - (40 - x) / 40.0);
            Thread.Sleep(1);
        }
        Thread.Sleep(holdTime);
        for (int  x = 0;  x <= 40;  x++) {
            target.Opacity = (40 - x) / 40.0;
            Thread.Sleep(1);
        }
    }

    public void SparkChange(Label target, string newContent) {
        for (int  x = 0;  x <= 40;  x++) {
            target.Opacity = (40 - x) / 40.0;
            Thread.Sleep(1);
        }
        target.Content = newContent;
        for (int  x = 0;  x <= 40;  x++) {
            target.Opacity =1 - (40 - x) / 40.0;
            Thread.Sleep(1);
        }
    }
    
    public void Fade(Control target,bool isFadeIn) {
        if (!isFadeIn) {
            for (int  x = 0;  x <= 40;  x++) {
                target.Opacity = (40 - x) / 40.0;
                Thread.Sleep(1);
            }
        } else {
            for (int x = 0; x <= 40; x++) {
                target.Opacity = 1 - (40 - x) / 40.0;
                Thread.Sleep(1);
            }
        }
    }
}