using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AVFoundation;
using Plugin.TextToSpeech.Abstractions;
using UIKit;

namespace DynaPad
{
	public class TextToSpeech : ITextToSpeech, IDisposable
	{
		private AVSpeechSynthesizer _speechSynthesizer;
		private bool _isSpeaking;

		public TextToSpeech()
		{
			_speechSynthesizer = new AVSpeechSynthesizer();
			_speechSynthesizer.DidFinishSpeechUtterance += speechSynthesizer_StoppedSpeechUtterance;
			_speechSynthesizer.DidCancelSpeechUtterance += speechSynthesizer_StoppedSpeechUtterance;
		}

		public event EventHandler<EventArgs> SpeechStopped = delegate { };

		public bool IsSpeaking { get { return _isSpeaking; } }

        public int MaxSpeechInputLength => throw new NotImplementedException();

        public void Speak(string text)
		{
			_isSpeaking = true;
			//var speechRate = UIDevice.CurrentDevice.CheckSystemVersion(8, 0) ? 8 : 4;
			var speechUtterance = new AVSpeechUtterance(text)
			{
				//Rate = AVSpeechUtterance.MaximumSpeechRate / speechRate,
				Rate = AVSpeechUtterance.DefaultSpeechRate,
				Voice = AVSpeechSynthesisVoice.FromLanguage("en-US"),
				Volume = 0.5f,
				PitchMultiplier = 1.0f
			};
			_speechSynthesizer.SpeakUtterance(speechUtterance);
		}

		private void speechSynthesizer_StoppedSpeechUtterance(object sender, AVSpeechSynthesizerUteranceEventArgs e)
		{
			_isSpeaking = false;
			OnSpeechStopped(e);

		}

		private void OnSpeechStopped(EventArgs e)
		{
			SpeechStopped(this, e);
		}

		public void StopSpeach()
		{
			_speechSynthesizer.StopSpeaking(AVSpeechBoundary.Immediate);
		}


		public void Dispose()
		{
			_speechSynthesizer.DidFinishSpeechUtterance -= speechSynthesizer_StoppedSpeechUtterance;
			_speechSynthesizer.DidCancelSpeechUtterance -= speechSynthesizer_StoppedSpeechUtterance;
		}

		public void Init()
		{
			throw new NotImplementedException();
		}

		public void Speak(string text, bool queue = false, CrossLocale? crossLocale = default(CrossLocale?), float? pitch = default(float?), float? speakRate = default(float?), float? volume = default(float?))
		{
			_isSpeaking = true;
			var speechRate = UIDevice.CurrentDevice.CheckSystemVersion(8, 0) ? 8 : 4;

			var speechUtterance = new AVSpeechUtterance(text)
			{
				Rate = AVSpeechUtterance.MaximumSpeechRate / speechRate,
				Voice = AVSpeechSynthesisVoice.FromLanguage("en-US"),
				Volume = 0.5f,
				PitchMultiplier = 1.0f
			};
			_speechSynthesizer.SpeakUtterance(speechUtterance);
		}

		public IEnumerable<CrossLocale> GetInstalledLanguages()
		{
			return null;
		}

        public Task Speak(string text, CrossLocale? crossLocale = default(CrossLocale?), float? pitch = default(float?), float? speakRate = default(float?), float? volume = default(float?), CancellationToken? cancelToken = default(CancellationToken?))
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<CrossLocale>> ITextToSpeech.GetInstalledLanguages()
        {
            throw new NotImplementedException();
        }
    }
}
