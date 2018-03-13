using System;
using UnityEngine;

namespace CareerManagerUI
{
	public class MenuToggle
	{
		private Rect size;

		private string description;

		public bool _state;

		private Action<bool> callback;

		private GameScenes[] _scenes;

		public bool state
		{
			get
			{
				return this._state;
			}
			set
			{
				this._state = value;
			}
		}

		public string getDescription
		{
			get
			{
				return this.description;
			}
		}

		public Rect getSize
		{
			get
			{
				return this.size;
			}
		}

		public MenuToggle(Rect sizeRect, bool defaultState, string desc, Action<bool> cback, GameScenes[] scenes)
		{
			this.initialize(sizeRect, defaultState, desc, cback, scenes);
		}

		public MenuToggle(Rect sizeRect, bool defaultState, string desc, Action<bool> cback)
		{
			this.initialize(sizeRect, defaultState, desc, cback, new GameScenes[]
			{
				GameScenes.SPACECENTER
			});
		}

		private void initialize(Rect sizeRect, bool defaultState, string desc, Action<bool> cback, GameScenes[] scenes)
		{
			this.size = new Rect(sizeRect);
			this._state = defaultState;
			this.description = desc;
			this.callback = cback;
			this._scenes = scenes;
		}

		public bool GetState()
		{
			return this._state;
		}

		public void draw(GameScenes scene)
		{
			bool flag = Array.FindIndex<GameScenes>(this._scenes, (GameScenes sc) => sc == scene) > -1;
			if (flag)
			{
				bool state = this._state;
				this._state = GUILayout.Toggle(this._state, this.description, new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(true)
				});
				bool flag2 = this._state != state;
				if (flag2)
				{
					this.callback(this._state);
				}
			}
		}
	}
}
