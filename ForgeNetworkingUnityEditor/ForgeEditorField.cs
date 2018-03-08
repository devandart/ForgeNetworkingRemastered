using UnityEditor;
using UnityEngine;
using System;

namespace BeardedManStudios.Forge.Networking.UnityEditor
{
	/// <summary>
	/// This is a editor field for the network object
	/// </summary>
	[Serializable]
	public class ForgeEditorField
	{
		public string FieldName;
		public bool CanRender = true;
		public bool Interpolate;
		public float InterpolateValue;
		public ForgeAcceptableFieldTypes FieldType;

        public ForgeCompressionSettings[] compressionSettings;
        
        public bool IsCompressable
        {
            get { return ForgeClassFieldValue.IsCompressable(FieldType); }
        }

        public float ElementHeight
        {
            get
            {
                var height = EditorGUIUtility.singleLineHeight + 2;
                if (IsCompressable)
                {
                    height *= compressionSettings.Length + 2;
                }
                return  height;
            }
        }

        public ForgeEditorField(string name = "", bool canRender = true, ForgeAcceptableFieldTypes type = ForgeAcceptableFieldTypes.BYTE, bool interpolate = false, float interpolateValue = 0f)
		{
			this.FieldName = name;
			this.FieldType = type;
			this.Interpolate = interpolate;
			this.InterpolateValue = interpolateValue;
			this.CanRender = canRender;

            InitializeCompressionSettings();
		}

		public void Render()
		{
			if (!CanRender)
				return;

			GUILayout.BeginHorizontal();
			FieldName = GUILayout.TextField(FieldName);
			FieldType = (ForgeAcceptableFieldTypes)EditorGUILayout.EnumPopup(FieldType, GUILayout.Width(75));
			//if (FieldType == ForgeAcceptableFieldTypes.Unknown) //Unsupported
			//{
			//	Debug.LogError("Can't set the type to unknown (Not Allowed)");
			//	FieldType = AcceptableTypes.INT;
			//}

			if (ForgeClassFieldValue.IsInterpolatable(FieldType))
			{
				GUI.color = Interpolate ? Color.white : Color.gray;
				if (GUILayout.Button("Interpolate", GUILayout.Width(100)))
					Interpolate = !Interpolate;

				if (Interpolate)
				{
					if (InterpolateValue == 0)
						InterpolateValue = ForgeNetworkingEditor.DEFAULT_INTERPOLATE_TIME;
					else
						InterpolateValue = EditorGUILayout.FloatField(InterpolateValue, GUILayout.Width(50));
				}
				else
				{
					InterpolateValue = 0;
					//InterpolateValue = ForgeNetworkingEditor.DEFAULT_INTERPOLATE_TIME;
				}
			}
		}

		public void Render(Rect rect, bool isActive, bool isFocused)
        {
            if (!CanRender)
                return;

            InitializeCompressionSettings();

            rect.y += 2;

            Rect changingRect = new Rect(rect.x, rect.y, rect.width * 0.3f, EditorGUIUtility.singleLineHeight);
            FieldName = EditorGUI.TextField(changingRect, FieldName);
            changingRect.x += rect.width * 0.3f + 5;
            FieldType = (ForgeAcceptableFieldTypes)EditorGUI.EnumPopup(changingRect, FieldType);
            if (ForgeClassFieldValue.IsInterpolatable(FieldType))
            {
                changingRect.x += rect.width * 0.3f + 10;
                changingRect.width = rect.width * 0.2f;
                Interpolate = EditorGUI.ToggleLeft(changingRect, "  Interpolate", Interpolate);

                if (Interpolate)
                {
                    if (InterpolateValue == 0)
                        InterpolateValue = ForgeNetworkingEditor.DEFAULT_INTERPOLATE_TIME;
                    else
                    {
                        changingRect.x += rect.width * 0.2f + 5;
                        changingRect.width = rect.width * 0.3f;
                        InterpolateValue = EditorGUI.FloatField(changingRect, InterpolateValue);
                    }
                }
                else
                    InterpolateValue = 0;
            }

            if (IsCompressable)
            {
                for (int i = 0; i < compressionSettings.Length; i++)
                {
                    var cs = compressionSettings[i];
                    var toggleText = "Compress";

                    if(compressionSettings.Length > 1)
                    {
                        switch (i)
                        {
                            case 0:
                                toggleText += " X";
                                break;
                            case 1:
                                toggleText += " Y";
                                break;
                            case 2:
                                toggleText += " Z";
                                break;
                            case 3:
                                toggleText += " W";
                                break;
                        }
                    }

                    changingRect.x = rect.x;
                    changingRect.width = rect.width * 0.1f;
                    changingRect.y += EditorGUIUtility.singleLineHeight + 2;
                    
                    cs.compress = EditorGUI.ToggleLeft(changingRect, toggleText, cs.compress);

                    if(cs.min >= cs.max)
                    {
                        cs.max = cs.min + 1;
                    }

                    if(cs.accuracy <= 0)
                    {
                        cs.accuracy = 0.01f;
                    }

                    changingRect.x += changingRect.width;
                    changingRect.width = rect.width * 0.035f;
                    EditorGUI.LabelField(changingRect, "  Min");
                    changingRect.x += changingRect.width;
                    changingRect.width = rect.width * 0.075f;
                    cs.min = EditorGUI.IntField(changingRect, cs.min);

                    changingRect.x += changingRect.width;
                    changingRect.width = rect.width * 0.035f;
                    EditorGUI.LabelField(changingRect, "  Max");
                    changingRect.x += changingRect.width;
                    changingRect.width = rect.width * 0.075f;
                    cs.max = EditorGUI.IntField(changingRect, cs.max);

                    changingRect.x += changingRect.width;
                    changingRect.width = rect.width * 0.06f;
                    EditorGUI.LabelField(changingRect, "  Accuracy");
                    changingRect.x += changingRect.width;
                    changingRect.width = rect.width * 0.075f;
                    cs.accuracy = EditorGUI.FloatField(changingRect, cs.accuracy);

                    changingRect.x += changingRect.width;
                    changingRect.width = rect.width * 0.2f;
                    EditorGUI.LabelField(changingRect, string.Format("  Used Bits: 16 (dummy value)"));
                }
            }
        }

        private void InitializeCompressionSettings()
        {
            if (IsCompressable)
            {
                if (compressionSettings == null || compressionSettings.Length != ForgeClassFieldValue.GetCompressionFieldCount(FieldType))
                {
                    compressionSettings = new ForgeCompressionSettings[ForgeClassFieldValue.GetCompressionFieldCount(FieldType)];
                    for (int i = 0; i < compressionSettings.Length; i++)
                    {
                        compressionSettings[i] = new ForgeCompressionSettings();
                    }
                }
            }
        }
    }
}