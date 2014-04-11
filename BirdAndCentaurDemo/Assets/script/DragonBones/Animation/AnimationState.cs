// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.1
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
using System;
using DragonBones.Objects;
using DragonBones.Events;
using Com.Viperstudio.Utils;
using System.Collections.Generic;

namespace DragonBones.Animation
{
	public class AnimationState
	{
		private static List<AnimationState> _pool = new List<AnimationState>();
		
		/** @private */
		public static AnimationState borrowObject()
		{
			if(_pool.Count == 0)
			{
				return new AnimationState();
			}
			AnimationState obj = _pool [_pool.Count - 1];
			_pool.RemoveAt(_pool.Count-1);
			return obj;
		}
		
		/** @private */
		public static void returnObject(AnimationState animationState)
		{
			animationState.clearVaribles();
			
			if(_pool.IndexOf(animationState) < 0)
			{
				_pool[_pool.Count] = animationState;
			}
		}
		
		/** @private */
		public static void clear()
		{
			int i = _pool.Count;
			while(i -- >0)
			{
				_pool[i].clearVaribles();
			}
			_pool.Clear();
			
			TimelineState.clear();
		}
		
		public bool TweenEnabled;
		public bool Blend;
		public string Group;
		public float Weight;
		
		/** @private */
		public Dictionary<string, TimelineState> _timelineStates;
		/** @private */
		public float _fadeWeight;
		
		private Armature _armature;
		private Frame _currentFrame;
		private Dictionary<string, int> _mixingTransforms;
		private int _fadeState;
		private float _fadeInTime;
		private float _fadeOutTime;
		private float _fadeOutBeginTime;
		private float _fadeOutWeight;
		private bool _fadeIn;
		private bool _fadeOut;
		private int _pauseBeforeFadeInCompleteState;
		
		private string _name;
		public string Name
		{
			get { return _name; }
		}
		
		private AnimationData _clip;
		public AnimationData Clip
		{
			get { return _clip; }
		}
		
		private int _loopCount;
		public int LoopCount
		{
			get { return _loopCount; }
		}
		
		private int _loop;
		public int Loop
		{
			get { return _loop; }
		}
		
		private uint _layer;
		public uint Layer
		{
			get { return _layer; }
		}
		
		private bool _isPlaying;
		public bool IsPlaying
		{
			get { return _isPlaying && !_isComplete; }
		}
		
		private bool _isComplete;
		public bool IsComplete
		{
			get { return _isComplete;  }
			set {  _isComplete = value;  }
		}
		
		public float FadeInTime
		{
			get { return _fadeInTime; }
		}
		
		private float _totalTime;
		public float TotalTime
		{
			get { return _totalTime; }
		}
		
		private float _currentTime;
		public float CurrentTime
		{
			get { return _currentTime; }
			set {
					if(value < 0 || float.IsNaN(value))
					{
						value = 0;
					}
					//
					_currentTime = value;

				}
		}

		
		private float _timeScale;
		public float TimeScale
		{
			get { return _timeScale; } 
			set {
					if(value < 0)
					{
						value = 0;
					}
					else if(float.IsNaN(value))
					{
						value = 1;
					}
					else if(_timeScale == float.PositiveInfinity)
					{
						//*
						_timeScale = 1;
					}
					_timeScale = value;
				}
		}

		
		public bool DisplayControl;
		
		public  AnimationState()
		{ 
			_timelineStates = new Dictionary<string, TimelineState>();
		}
		
		/** @private */
		public void fadeIn(Armature armature, AnimationData clip, float fadeInTime, float timeScale, int loop, uint layer, bool displayControl, bool pauseBeforeFadeInComplete)
		{

			_armature = armature;
			_clip = clip;
			_name = _clip.Name;
			_layer = layer;
			
			_totalTime = _clip.Duration;
			if(Math.Round(_clip.Duration * _clip.FrameRate) < 2 || timeScale == float.PositiveInfinity)
			{
				_timeScale = 1;
				_currentTime = _totalTime;
				if(_loop >= 0)
				{
					_loop = 1;
				}
				else
				{
					_loop = -1;
				}
			}
			else
			{
				_timeScale = timeScale;
				_currentTime = 0;
				_loop = loop;
			}
			
			if(pauseBeforeFadeInComplete)
			{
				_pauseBeforeFadeInCompleteState = -1;
			}
			else
			{
				_pauseBeforeFadeInCompleteState = 1;
			}
			
			_fadeInTime = fadeInTime * _timeScale;
			
			
			_loopCount = -1;
			_fadeState = 1;
			_fadeOutBeginTime = 0;
			_fadeOutWeight = -1;
			_fadeWeight = 0;
			_isPlaying = true;
			_isComplete = false;
			_fadeIn = true;
			_fadeOut = false;
			
			this.DisplayControl = displayControl;
			
			Weight = 1;
			Blend = true;
			TweenEnabled = true;
			
			updateTimelineStates();

		}
		
		public void FadeOut(float fadeOutTime, bool pause = false)
		{
			if(_armature==null || _fadeOutWeight >= 0)
			{
				return;
			}
			_fadeState = -1;
			_fadeOutWeight = _fadeWeight;
			_fadeOutTime = fadeOutTime * _timeScale;
			_fadeOutBeginTime = _currentTime;
			
			_isPlaying = !pause;
			_fadeOut = true;
			DisplayControl = false;
			
			foreach(KeyValuePair<string, TimelineState> timelineState in _timelineStates)
			{
				(timelineState.Value as TimelineState).FadeOut();
			}
		}
		
		public void Play()
		{
			_isPlaying = true;
		}
		
		public void Stop()
		{
			_isPlaying = false;
		}
		
		public int GetMixingTransform(string timelineName)
		{
			if(_mixingTransforms!=null)
			{
				return _mixingTransforms[timelineName];
			}
			return -1;
		}
		
		public void AddMixingTransform(string timelineName, int type = 2, bool recursive = true)
		{
			if(_clip!=null && _clip.GetTimeline(timelineName)!=null)
			{
				if(_mixingTransforms == null)
				{
					_mixingTransforms = new Dictionary<string, int>();
				}
				if(recursive)
				{
					int i = _armature._boneList.Count;
					Bone bone = null;
					Bone currentBone = null;
					while(i -- >0)
					{
						bone = _armature._boneList[i];
						if(bone.Name == timelineName)
						{
							currentBone = bone;
						}
						if(currentBone!=null && (currentBone == bone || currentBone.Contains(bone)))
						{
							_mixingTransforms[bone.Name] = type;
						}
					}
				}
				else
				{
					_mixingTransforms[timelineName] = type;
				}
				
				updateTimelineStates();
			}
			else
			{
				throw new ArgumentException();
			}
		}
		
		public void RemoveMixingTransform(string timelineName = null, bool recursive = true)
		{
			if(timelineName!=null)
			{
				if(recursive)
				{
					int i = _armature._boneList.Count;
					Bone bone = null;
					Bone currentBone = null;
					while(i -- >0)
					{
						bone = _armature._boneList[i];
						if(bone.Name == timelineName)
						{
							currentBone = bone;
						}
						if(currentBone!=null && (currentBone == bone || currentBone.Contains(bone)))
						{
						     _mixingTransforms.Remove(bone.Name);
						}
					}
				}
				else
				{
				     _mixingTransforms.Remove(timelineName);
				}
				bool hasMixing = false;
				foreach(KeyValuePair<string, int> timelineName1 in _mixingTransforms)
				{
					hasMixing = true;
					break;
				}
				if(!hasMixing)
				{
					_mixingTransforms = null;
				}
			}
			else
			{
				_mixingTransforms = null;
			}
			
			updateTimelineStates();
		}
		
		public bool AdvanceTime(float passedTime)
		{
			AnimationEvent evt;
			bool isComplete = false;
			
			if(_fadeIn)
			{	
				_fadeIn = false;
				_armature.Animation.setActive(this, true);
				if(_armature.HasEventListener(AnimationEvent.FADE_IN))
				{
					evt = new AnimationEvent(AnimationEvent.FADE_IN);
					evt.AnimationState = this;
					_armature._eventList.Add(evt);
				}
			}
			
			if(_fadeOut)
			{	
				_fadeOut = false;
				_armature.Animation.setActive(this, true);
				if(_armature.HasEventListener(AnimationEvent.FADE_OUT))
				{
					evt = new AnimationEvent(AnimationEvent.FADE_OUT);
					evt.AnimationState = this;
					_armature._eventList.Add(evt);
				}
			}

			//Logger.Log("count " + passedTime + "  " + _timeScale );

			_currentTime += passedTime * _timeScale;
			
			if(_isPlaying && !_isComplete && _pauseBeforeFadeInCompleteState!=0)
			{
				float progress;
				int currentLoopCount;
				if(_pauseBeforeFadeInCompleteState == -1)
				{
					_pauseBeforeFadeInCompleteState = 0;
					progress = 0;
					currentLoopCount = (int)progress;
				}
				else
				{
					progress = _currentTime / _totalTime;
					//update loopCount

					currentLoopCount = (int)progress;

					if(currentLoopCount != _loopCount)
					{
						if(_loopCount == -1)
						{

							_armature.Animation.setActive(this, true);
							if(_armature.HasEventListener(AnimationEvent.START))
							{
								evt = new AnimationEvent(AnimationEvent.START);
								evt.AnimationState = this;
								_armature._eventList.Add(evt);
							}
						}
						_loopCount = currentLoopCount;

						if(_loopCount!=0)
						{

							if(_loop !=0 && _loopCount * _loopCount >= _loop * _loop - 1)
							{
								isComplete = true;
								progress = 1;
								currentLoopCount = 0;
								if(_armature.HasEventListener(AnimationEvent.COMPLETE))
								{
									evt = new AnimationEvent(AnimationEvent.COMPLETE);
									evt.AnimationState = this;
									_armature._eventList.Add(evt);
								}
							}
							else
							{
								if(_armature.HasEventListener(AnimationEvent.LOOP_COMPLETE))
								{
									evt = new AnimationEvent(AnimationEvent.LOOP_COMPLETE);
									evt.AnimationState = this;
									_armature._eventList.Add(evt);
								}
							}
						}
					}
				}
				
				
				foreach(KeyValuePair<string, TimelineState> timeline in _timelineStates)
				{
					//Logger.Log(timeline.Key);
					(timeline.Value as TimelineState).Update(progress);
					//break;
				}
				

				if(_clip.FrameList.Count > 0)
				{
					float playedTime = _totalTime * (progress - currentLoopCount);
					bool isArrivedFrame = false;
					int frameIndex;

					while(_currentFrame ==null || playedTime > _currentFrame.Position + _currentFrame.Duration || playedTime < _currentFrame.Position)
					{
						if(isArrivedFrame)
						{
							_armature.arriveAtFrame(_currentFrame, null, this, true);
						}
						isArrivedFrame = true;
						if(_currentFrame!=null)
						{
							frameIndex = _clip.FrameList.IndexOf(_currentFrame);
							frameIndex ++;
							if(frameIndex >= _clip.FrameList.Count)
							{
								frameIndex = 0;
							}
							_currentFrame = _clip.FrameList[frameIndex];
						}
						else
						{
							_currentFrame = _clip.FrameList[0];
						}
					}

					if(isArrivedFrame)
					{
						_armature.arriveAtFrame(_currentFrame, null, this, false);
					}

				}
			}
			
			//update weight and fadeState
			if(_fadeState > 0)
			{
				if(_fadeInTime == 0)
				{
					_fadeWeight = 1;
					_fadeState = 0;
					_pauseBeforeFadeInCompleteState = 1;
					_armature.Animation.setActive(this, false);
					if(_armature.HasEventListener(AnimationEvent.FADE_IN_COMPLETE))
					{
						evt = new AnimationEvent(AnimationEvent.FADE_IN_COMPLETE);
						evt.AnimationState = this;
						_armature._eventList.Add(evt);
					}
				}
				else
				{
					_fadeWeight = _currentTime / _fadeInTime;
					if(_fadeWeight >= 1)
					{
						_fadeWeight = 1;
						_fadeState = 0;
						if(_pauseBeforeFadeInCompleteState == 0)
						{
							_currentTime -= _fadeInTime;
						}
						_pauseBeforeFadeInCompleteState = 1;
						_armature.Animation.setActive(this, false);
						if(_armature.HasEventListener(AnimationEvent.FADE_IN_COMPLETE))
						{
							evt = new AnimationEvent(AnimationEvent.FADE_IN_COMPLETE);
							evt.AnimationState = this;
							_armature._eventList.Add(evt);
						}
					}
				}
			}
			else if(_fadeState < 0)
			{
				if(_fadeOutTime == 0)
				{
					_fadeWeight = 0;
					_fadeState = 0;
					_armature.Animation.setActive(this, false);
					if(_armature.HasEventListener(AnimationEvent.FADE_OUT_COMPLETE))
					{
						evt = new AnimationEvent(AnimationEvent.FADE_OUT_COMPLETE);
						evt.AnimationState = this;
						_armature._eventList.Add(evt);
					}
					return true;
				}
				else
				{
					_fadeWeight = (1 - (_currentTime - _fadeOutBeginTime) / _fadeOutTime) * _fadeOutWeight;
					if(_fadeWeight <= 0)
					{
						_fadeWeight = 0;
						_fadeState = 0;
						_armature.Animation.setActive(this, false);
						if(_armature.HasEventListener(AnimationEvent.FADE_OUT_COMPLETE))
						{
							evt = new AnimationEvent(AnimationEvent.FADE_OUT_COMPLETE);
							evt.AnimationState = this;
							_armature._eventList.Add(evt);
						}
						return true;
					}
				}
			}
			
			if(isComplete)
			{

				_isComplete = true;
				if(_loop < 0)
				{
					float r = 0f;
					if(!float.IsNaN(_fadeInTime)&&_fadeInTime!=0) r = _fadeInTime;
					if(!float.IsNaN(_fadeOutWeight)&&_fadeOutWeight!=0) r = _fadeOutWeight;
				
					FadeOut(r / _timeScale, true);
				}
				else
				{
					_armature.Animation.setActive(this, false);
				}
			}
			
			return false;
		}
		
		private void updateTimelineStates()
		{
			if(_mixingTransforms!=null)
			{
				foreach(KeyValuePair<string, TimelineState> timelineState in _timelineStates)
				{
					if(timelineState.Value == null)
					{
						removeTimelineState(timelineState.Key);
					}
				}
				
				foreach(KeyValuePair<string, int> timelineState in _mixingTransforms)
				{
					if(timelineState.Value == 0)
					{
						addTimelineState(timelineState.Key);
					}
				}
			}
			else
			{
				foreach(KeyValuePair<string, TransformTimeline>  timelineState in _clip.Timelines)
				{
					if(timelineState.Value!=null)
					{
						addTimelineState(timelineState.Key);
					}
				}
			}
		}
		
		private void addTimelineState(string timelineName)
		{
			Bone bone = _armature.GetBone(timelineName);
			if(bone!=null)
			{
				TimelineState timelineState = TimelineState.borrowObject();
				TransformTimeline timeline = _clip.GetTimeline(timelineName);
				timelineState.FadeIn(bone, this, timeline);
				_timelineStates[timelineName] = timelineState;
			}
		}
		
		private void removeTimelineState(string timelineName)
		{
			TimelineState.returnObject(_timelineStates[timelineName] as TimelineState);
			_timelineStates.Remove(timelineName);
		}
		
		private void clearVaribles()
		{
			_armature = null;
			_currentFrame = null;
			_clip = null;
			_mixingTransforms = null;
			
			foreach(KeyValuePair<string, TimelineState> timelineState in _timelineStates)
			{
				removeTimelineState(timelineState.Key);
			}
		}
	}
}

