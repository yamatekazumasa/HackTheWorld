﻿using System.Diagnostics;
using System.Drawing;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    /// <summary>
    /// アニメーションするオブジェクト。
    /// コンストラクタまたは Initialize メソッドの中で、
    /// `this.SetAnimation()` をすることでアニメーションをセットできる。
    /// </summary>
    public interface IAnimatable
    {
        float X { get; set; }
        float Y { get; set; }
        float W { get; set; }
        float H { get; set; }
        Animation Anim { get; set; }
    }

    public static partial class Extensions
    {
        public static void SetAnimation(this IAnimatable self, Image[] images, float[] timeline)
        {
            self.Anim = new Animation(self, images, timeline);
        }
    }

    /// <summary>
    /// アニメーション
    /// </summary>
    public class Animation
    {
        private float _elapsedTime;
        private readonly Image[] _images;
        private readonly Image[] _flippedImages;
        private readonly float[] _timeline;
        private readonly bool _loop;
        private int _current;
        private readonly IAnimatable _subject;
        public bool IsPlaying { get; private set; }

        public Animation(IAnimatable obj, Image[] images, float[] timeline)
        {
            Debug.Assert(images.Length == timeline.Length, "アニメーションの画像と時間情報の数が一致しません。");
            _images = images;
            _flippedImages = new Image[images.Length];
            for (int i = 0; i < images.Length; i++)
            {
                _flippedImages[i] = (Image)images[i].Clone();
                _flippedImages[i].RotateFlip(RotateFlipType.RotateNoneFlipX);
            }
            _timeline = timeline;
            _loop = true;
            _current = 0;
            _subject = obj;
        }

        /// <summary>
        /// アニメーションを開始する。
        /// </summary>
        public void Start()
        {
            _elapsedTime = 0.0f;
            _current = 0;
            IsPlaying = true;
        }

        /// <summary>
        /// アニメーションを停止する。
        /// </summary>
        public void Stop()
        {
            IsPlaying = false;
        }

        /// <summary>
        /// 最初からアニメーションをする。
        /// </summary>
        public void Rewind()
        {
            _elapsedTime = 0.0f;
            _current = 0;
        }

        /// <summary>
        /// アニメーションを dt 進める。
        /// </summary>
        public void Advance(float dt)
        {
            _elapsedTime += dt;
            if (_elapsedTime > _timeline[_current])
            {
                _current++;
                if (_current == _timeline.Length && _loop) Rewind();
            }
        }

        public void Draw()
        {
            GraphicsContext.DrawImage(_images[_current], _subject.X, _subject.Y, _subject.W, _subject.H);
        }

        public void Draw(bool flipped)
        {
            GraphicsContext.DrawImage(flipped ? _flippedImages[_current] : _images[_current], _subject.X, _subject.Y, _subject.W, _subject.H);
        }
    }
}
