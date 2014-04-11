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
using DragonBones.Core;
using DragonBones.Events;
using DragonBones.Objects;
using DragonBones.Display;
using DragonBones.Animation;
using Com.Viperstudio.Events;
using Com.Viperstudio.Utils;


namespace DragonBones
{

	
	public class Armature :EventDispatcher, IAnimatable
	{

		private static SoundEventManager _soundManager = SoundEventManager.Instance;
		
		private static List<Object> _helpArray = new List<Object>();
		
		/**
		 * The name of this DBObject instance's Armature instance.
		 */
		public string Name;
		
		/**
		 * An object that can contain any user extra data.
		 */
		public Object UserData;
		
		/** @private */
		public bool _slotsZOrderChanged;
		/** @private */
		public List<Slot> _slotList;
		/** @private */
		public List<Bone> _boneList;
		/** @private */
		public List<Event> _eventList;
		
		/** @private */
		protected bool _needUpdate;
		
		/** @private */
		protected Object _display;
		/**
		 * Instance type of this object varies from flash.display.DisplayObject to startling.display.DisplayObject and subclasses.
		 */
		public Object Display{
			get {  return _display;  }
		}

		
		/** @private */
		protected Animation.Animation _animation;
		/**
		 * An Animation instance
		 * @see dragonBones.animation.Animation
		 */
		public Animation.Animation Animation
		{
			get {  return _animation; }
		}
		
		/**
		 * Creates a Armature blank instance.
		 * @param	Instance type of this object varies from flash.display.DisplayObject to startling.display.DisplayObject and subclasses.
		 */
		public Armature(Object display)
		{

			_display = display;

			_animation = new Animation.Animation(this);
			_slotsZOrderChanged = false;
			
			_slotList = new List<Slot>();
			//_slotList.fixed = true;
			_boneList = new List<Bone>();
			//_boneList.fixed = true;
			_eventList = new List<Event>();
			
			_needUpdate = false;
		}
		
		/**
		 * Cleans up any resources used by this DBObject instance.
		 */
		public void Dispose()
		{

			if(_animation==null)
			{
				return;
			}
			
			UserData = null;
			
			_animation.Dispose();
			
			foreach(Slot slot in _slotList)
			{
				slot.Dispose();
			}
			
			foreach(Bone bone in _boneList)
			{
				bone.Dispose();
			}
			
			//_slotList.fixed = false;
			_slotList.Clear();
			//_boneList.fixed = false;
			_boneList.Clear();
			_eventList.Clear();
			
			_animation = null;
			_slotList = null;
			_boneList = null;
			_eventList = null;
			
			//_display = null;
		}
		
		public void InvalidUpdate()
		{
			_needUpdate = true;
		}
		
		/**
		 * Update the animation using this method typically in an ENTERFRAME Event or with a Timer.
		 * @param	The amount of second to move the playhead ahead.
		 */
		public void AdvanceTime(float passedTime)
		{

			int i;
			Slot slot;
			Armature childArmature;
			if(_animation.IsPlaying || _needUpdate)
			{	

				_needUpdate = false;
				_animation.AdvanceTime(passedTime);
				passedTime *= _animation.TimeScale;
				
				i = _boneList.Count;
			
				while(i -- > 0)
				{

					_boneList[i].update();

				}

				i = _slotList.Count;
				while(i -- >0)
				{


					slot = _slotList[i];
					slot.update();

					if(slot._isDisplayOnStage)
					{
						childArmature = slot.ChildArmature;
						if(childArmature!=null)
						{
							childArmature.AdvanceTime(passedTime);
						}
					}


				}

				(_display as UnityArmatureDisplay).UpdateDisplay(_slotList);

				if(_slotsZOrderChanged)
				{
					UpdateSlotsZOrder();
					
					if(this.HasEventListener(ArmatureEvent.Z_ORDER_UPDATED))
					{
						this.DispatchEvent(new ArmatureEvent(ArmatureEvent.Z_ORDER_UPDATED));
					}
				}
				
				if(_eventList.Count!=0)
				{
					foreach(Event evt in _eventList)
					{
						this.DispatchEvent(evt);
					}
					_eventList.Clear();
				}

			}
			else
			{

				passedTime *= _animation.TimeScale;
				i = _slotList.Count;
				while(i -- >0)
				{
					slot = _slotList[i];
					if(slot._isDisplayOnStage)
					{
						childArmature = slot.ChildArmature;
						if(childArmature!=null)
						{
							childArmature.AdvanceTime(passedTime);
						}
					}
				}
			}
		}
		
		
		/**
		 * Get all Slot instance associated with this armature.
		 * @return A Vector.&lt;Slot&gt; instance.
		 * @see dragonBones.Slot
		 */
		public List<Slot> GetSlots(bool returnCopy = true)
		{
			return _slotList;//returnCopy?_slotList.MemberwiseClone():_slotList;
		}
		
		/**
		 * Get all Bone instance associated with this armature.
		 * @return A Vector.&lt;Bone&gt; instance.
		 * @see dragonBones.Bone
		 */
		public List<Bone> GetBones(bool returnCopy = true)
		{
			return _boneList;//returnCopy?_boneList.MemberwiseClone():_boneList;
		}
		
		/**
		 * Retrieves a Slot by name
		 * @param	The name of the Bone to retrieve.
		 * @return A Slot instance or null if no Slot with that name exist.
		 * @see dragonBones.Slot
		 */
		public Slot GetSlot(string slotName)
		{
			int i = _slotList.Count;
			while(i -- >0)
			{
				if(_slotList[i].Name == slotName)
				{
					return _slotList[i];
				}
			}
			return null;
		}
		
		/**
		 * Gets the Slot associated with this DisplayObject.
		 * @param	Instance type of this object varies from flash.display.DisplayObject to startling.display.DisplayObject and subclasses.
		 * @return A Slot instance.
		 * @see dragonBones.Slot
		 */
		public Slot GetSlotByDisplay(Object display)
		{
			if(display!=null)
			{
				int i = _slotList.Count;
				while(i -- >0)
				{
					if(_slotList[i].Display == display)
					{
						return _slotList[i];
					}
				}
			}
			return null;
		}
		
		/**
		 * Remove a Slot instance from this Armature instance.
		 * @param	The Slot instance to remove.
		 * @see dragonBones.Slot
		 */
		public void RemoveSlot(Slot slot)
		{
			if(slot==null)
			{
				throw new ArgumentException();
			}
			
			if(_slotList.IndexOf(slot) >= 0)
			{
				slot.Parent.RemoveChild(slot);
			}
			else
			{
				throw new ArgumentException();
			}
		}
		
		/**
		 * Remove a Slot instance from this Armature instance.
		 * @param	The name of the Slot instance to remove.
		 * @see dragonBones.Slot
		 */
		public void RemoveSlotByName(string slotName)
		{
			if(slotName == null)
			{
				return;
			}
			
			Slot slot = GetSlot(slotName);
			if(slot!=null)
			{
				RemoveSlot(slot);
			}
		}
		
		/**
		 * Retrieves a Bone by name
		 * @param	The name of the Bone to retrieve.
		 * @return A Bone instance or null if no Bone with that name exist.
		 * @see dragonBones.Bone
		 */
		public Bone GetBone(string boneName)
		{
			int i = _boneList.Count;
			while(i -- >0)
			{
				if(_boneList[i].Name == boneName)
				{
					return _boneList[i];
				}
			}
			return null;
		}
		
		/**
		 * Gets the Bone associated with this DisplayObject.
		 * @param	Instance type of this object varies from flash.display.DisplayObject to startling.display.DisplayObject and subclasses.
		 * @return A Bone instance.
		 * @see dragonBones.Bone
		 */
		public Bone GetBoneByDisplay(Object display)
		{
			Slot slot = GetSlotByDisplay(display);
			return slot!=null?slot.Parent:null;
		}
		
		/**
		 * Remove a Bone instance from this Armature instance.
		 * @param	The Bone instance to remove.
		 * @see	dragonBones.Bone
		 */
		public void RemoveBone(Bone bone)
		{
			if(bone==null)
			{
				throw new ArgumentException();
			}
			
			if(_boneList.IndexOf(bone) >= 0)
			{
				if(bone.Parent!=null)
				{
					bone.Parent.RemoveChild(bone);
				}
				else
				{
					bone.setArmature(null);
				}
			}
			else
			{
				throw new ArgumentException();
			}
		}
		
		/**
		 * Remove a Bone instance from this Armature instance.
		 * @param	The name of the Bone instance to remove.
		 * @see dragonBones.Bone
		 */
		public void RemoveBoneByName(string boneName)
		{
			if(boneName==null)
			{
				return;
			}
			
			Bone bone = GetBone(boneName);
			if(bone!=null)
			{
				RemoveBone(bone);
			}
		}
		
		
		/**
		 * Add a DBObject instance to this Armature instance.
		 * @param	A DBObject instance
		 * @param	(optional) The parent's name of this DBObject instance.
		 * @see dragonBones.core.DBObject
		 */
		public void AddChild(DBObject obj, string parentName = null)
		{
			if(obj == null)
			{
				throw new ArgumentException();
			}
			
			if(parentName != null)
			{
				Bone boneParent = GetBone(parentName);
				if (boneParent != null)
				{
					boneParent.AddChild(obj);
				}
				else
				{
					throw new ArgumentException();
				}
			}
			else
			{
				if(obj.Parent!=null)
				{
					obj.Parent.RemoveChild(obj);
				}
				obj.setArmature(this);
			}
		}
		
		/**
		 * Add a Bone instance to this Armature instance.
		 * @param	A Bone instance
		 * @param	(optional) The parent's name of this Bone instance.
		 * @see dragonBones.Bone
		 */
		public void AddBone(Bone bone, string parentName = null)
		{
			AddChild(bone, parentName);
		}

		/**
		 * Update the z-order of the display. 
		 */
		public void UpdateSlotsZOrder()
		{
			//_slotList.fixed = false;
			_slotList.Sort(sortSlot);
			//_slotList.fixed = true;
			int i = _slotList.Count;
			Slot slot;
			while(i -- >0)
			{
				slot = _slotList[i];
				if(slot._isDisplayOnStage)
				{
					slot._displayBridge.AddDisplay(Display, slot.ZOrder);
				}
			}
			
			_slotsZOrderChanged = false;
		}
		
		/** @private */
		public void addDBObject(DBObject obj)
		{
			if(obj is Slot)
			{
				Slot slot = obj as Slot;
				if(_slotList.IndexOf(slot) < 0)
				{
					//_slotList.fixed = false;
					_slotList.Add (slot);
					//_slotList.fixed = true;
				}
			}
			else if(obj is Bone)
			{
				Bone bone = obj as Bone;
				if(_boneList.IndexOf(bone) < 0)
				{
					//_boneList.fixed = false;
					_boneList.Add(bone);
					sortBoneList();
					//_boneList.fixed = true;
				}
			}
		}
		
		/** @private */
		public void removeDBObject(DBObject obj)
		{
			int index;
			if(obj is Slot)
			{
				Slot slot = obj as Slot;
				index = _slotList.IndexOf(slot);
				if(index >= 0)
				{
					//_slotList.fixed = false;
					_slotList.RemoveAt(index);
					//_slotList.fixed = true;
				}
			}
			else if(obj is Bone)
			{
				Bone bone = obj as Bone;
				index = _boneList.IndexOf(bone);
				if(index >= 0)
				{
					//_boneList.fixed = false;
					_boneList.RemoveAt(index);
					//_boneList.fixed = true;
				}
			}
		}
		
		/** @private */
		public void sortBoneList()
		{
			int i = _boneList.Count;
			if(i == 0)
			{
				return;
			}
			_helpArray.Clear();
			int level;
			Bone bone;
			Bone boneParent;
			while(i -- >0)
			{
				level = 0;
				bone = _boneList[i];
				boneParent = bone;
				while(boneParent != null)
				{
					level ++;
					boneParent = boneParent.Parent;
				}
				_helpArray.Insert(0, new HelpObject());
				(_helpArray[0] as HelpObject).Level =  level;
				(_helpArray[0] as HelpObject).Bone = bone;
			}


			HelpComparer hc = new HelpComparer();
			_helpArray.Sort(hc);
			
			i = _helpArray.Count;
			while(i -- >0)
			{
				_boneList[i] = (_helpArray[i] as HelpObject).Bone;
			}
			_helpArray.Clear();
		}

		
		/** @private */
		public void arriveAtFrame(Frame frame, TimelineState timelineState, AnimationState animationState, bool isCross)
		{
			if(frame.Evt!=null && this.HasEventListener(FrameEvent.ANIMATION_FRAME_EVENT))
			{
				FrameEvent frameEvent = new FrameEvent(FrameEvent.ANIMATION_FRAME_EVENT);
				frameEvent.AnimationState = animationState;
				frameEvent.FrameLabel = frame.Evt;
				_eventList.Add(frameEvent);
			}
			
			if(frame.Sound!=null && _soundManager.HasEventListener(SoundEvent.SOUND))
			{
				SoundEvent soundEvent = new SoundEvent(SoundEvent.SOUND);
				soundEvent.Armature = this;
				soundEvent.AnimationState = animationState;
				soundEvent.Sound = frame.Sound;
				_soundManager.DispatchEvent(soundEvent);
			}
			
			if(frame.Action!=null)
			{
				if(animationState.IsPlaying)
				{
					Animation.GotoAndPlay(frame.Action);
				}
			}
		}
		
		private int sortSlot(Slot slot1, Slot slot2)
		{
			return slot1.ZOrder < slot2.ZOrder?1: -1;
		}


	}

	public class HelpObject {
		public int Level;
		public Bone Bone;


	}

	public class HelpComparer: IComparer<Object>
	{
		public int Compare(Object x, Object y)
		{
	
			return ((y as HelpObject).Level.CompareTo((x as HelpObject).Level));
			
			//if (x.Level > y.Level) return (int)(-1);
			//else if(x.Level == y.Level) return (int)0;
			//else return (int)1;
		}
	}

}

