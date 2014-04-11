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
using System.Collections.Generic;
namespace DragonBones.Objects
{
	public class Timeline
	{
		private List<Frame> _frameList;
		public List<Frame> FrameList
		{
			get { return _frameList; }
		}
		
		private float _duration;
		public float Duration
		{
			get { return _duration; }
			set { _duration = value >= 0 ? value : 0; } 
		}
	
		private float _scale;
		public float Scale
		{
			get { return _scale; }
			set { _scale = value >= 0 ? value : 1;}
		}

		public Timeline()
		{
			_frameList = new List<Frame>();
			_duration = 0;
			_scale = 1;
		}
		
		public virtual void Dispose()
		{
			int i = _frameList.Count;
			while(i -- > 0)
			{
				_frameList[i].Dispose();
			}

		    _frameList.Clear();
			_frameList = null;
		}
		
		public void AddFrame(Frame frame)
		{
			if(frame == null)
			{
				throw new ArgumentException();
			}
			
			if(_frameList.IndexOf(frame) < 0)
			{

				_frameList.Add(frame);
			
			}
			else
			{
				throw new ArgumentException();
			}
		}
	}
}

