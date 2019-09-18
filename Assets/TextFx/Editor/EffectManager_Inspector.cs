using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(EffectManager))]
public class EffectManager_Inspector : Editor
{
	bool m_previewing_anim = false;			// Denotes whether the animation is currently being previewed in the editor
	bool m_paused = false;
	EffectManager font_manager;
	float m_old_time = 0;
	
    void OnEnable()
    {
		EditorApplication.update += UpdateFunction;
    }
	
	void OnDisable()
	{
		EditorApplication.update -= UpdateFunction;
	}
	
	private void UpdateFunction()
	{
		if(m_previewing_anim && !m_paused)
		{
			if(font_manager == null)
			{
				font_manager = (EffectManager)target;
			}
			
			if(!font_manager.UpdateAnimation(Time.realtimeSinceStartup - m_old_time))
			{
				m_previewing_anim = false;
			}
			
			m_old_time = Time.realtimeSinceStartup;
		}
	}
	
	public override void OnInspectorGUI ()
	{
		font_manager = (EffectManager)target;
		
		string old_text = font_manager.m_text;
		Vector2 old_px_offset = font_manager.m_px_offset;
		float old_character_size = font_manager.m_character_size;
		TextDisplayAxis old_display_axis = font_manager.m_display_axis;
		TextAnchor old_text_anchor = font_manager.m_text_anchor;
		
		DrawDefaultInspector();
		
		EditorGUILayout.Separator();
		EditorGUILayout.Separator();
		
		EditorGUILayout.BeginHorizontal();
			if(GUILayout.Button(!m_previewing_anim || m_paused ? "Play" : "Pause"))
			{
				if(m_previewing_anim)
				{
					m_paused = !m_paused;
				}
				else
				{
					m_previewing_anim= true;
					
					font_manager.PlayAnimation();
					m_paused = false;
				}
			
				m_old_time = Time.realtimeSinceStartup;
			}
			if(GUILayout.Button("Reset"))
			{
				font_manager.ResetAnimation();
				m_paused = false;
			}
		
		EditorGUILayout.EndHorizontal();
		
		if (GUI.changed)
		{
			if(!old_text.Equals(font_manager.m_text)
				|| old_px_offset != font_manager.m_px_offset
				|| old_display_axis != font_manager.m_display_axis
				|| old_text_anchor != font_manager.m_text_anchor
				|| old_character_size != font_manager.m_character_size)
			{
				font_manager.SetText(font_manager.m_text);
			}
			font_manager.SetAnimationState(0, 0);
		}
		
		
		if (GUILayout.Button("Open TextFX Manager"))
		{
			EditorWindow.GetWindow(typeof(TextEffectsManager));
		}
	}
}