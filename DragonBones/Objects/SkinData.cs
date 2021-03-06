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
	public class SkinData
	{
		public string Name;
		
		private List<SlotData> _slotDataList;
		public List<SlotData> SlotDataList
		{
			get { return _slotDataList; }
		}
		
		public SkinData()
		{
			_slotDataList = new List<SlotData>();
		}
		
		public void Dispose()
		{
			int i = _slotDataList.Count;
			while(i -- >0)
			{
				_slotDataList[i].Dispose();
			}

			_slotDataList.Clear();

		}
		
		public SlotData getSlotData(string slotName)
		{
			int i = _slotDataList.Count;
			while(i -- >0)
			{
				if(_slotDataList[i].Name == slotName)
				{
					return _slotDataList[i];
				}
			}
			return null;
		}
		
		public void AddSlotData(SlotData slotData)
		{
			if(slotData == null)
			{
				throw new ArgumentException();
			}
			
			if (_slotDataList.IndexOf(slotData) < 0)
			{

				_slotDataList.Add(slotData);

			}
			else
			{
				throw new ArgumentException();
			}
		}
	}
}

