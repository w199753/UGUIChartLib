using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

[System.Serializable]
public class VertexColour
{
	public Color top_left = Color.white;
	public Color top_right = Color.white;
	public Color bottom_right = Color.white;
	public Color bottom_left = Color.white;
	
	public VertexColour()
	{
		
	}
	
	public VertexColour(Color init_color)
	{
		top_left = init_color;
		top_right = init_color;
		bottom_right = init_color;
		bottom_left = init_color;
	}
	
	public VertexColour(Color tl_colour, Color tr_colour, Color br_colour, Color bl_colour)
	{
		top_left = tl_colour;
		top_right = tr_colour;
		bottom_right = br_colour;
		bottom_left = bl_colour;
	}
	
	public VertexColour(VertexColour vert_col)
	{
		top_left = vert_col.top_left;
		top_right = vert_col.top_right;
		bottom_right = vert_col.bottom_right;
		bottom_left = vert_col.bottom_left;
	}
	
	public VertexColour Clone()
	{
		VertexColour vertex_col = new VertexColour();
		vertex_col.top_left = top_left;
		vertex_col.top_right = top_right;
		vertex_col.bottom_right = bottom_right;
		vertex_col.bottom_left = bottom_left;
		
		return vertex_col;
	}
	
	public VertexColour Add(VertexColour vert_col)
	{
		VertexColour v_col = new VertexColour();
		v_col.bottom_left = bottom_left + vert_col.bottom_left;
		v_col.bottom_right = bottom_right + vert_col.bottom_right;
		v_col.top_left = top_left + vert_col.top_left;
		v_col.top_right = top_right + vert_col.top_right;
		
		return v_col;
	}
	
	public VertexColour Sub(VertexColour vert_col)
	{
		VertexColour v_col = new VertexColour();
		v_col.bottom_left = bottom_left - vert_col.bottom_left;
		v_col.bottom_right = bottom_right - vert_col.bottom_right;
		v_col.top_left = top_left - vert_col.top_left;
		v_col.top_right = top_right - vert_col.top_right;
		
		return v_col;
	}
	
	public VertexColour Multiply(float factor)
	{
		VertexColour v_col = new VertexColour();
		v_col.bottom_left = bottom_left * factor;
		v_col.bottom_right = bottom_right * factor;
		v_col.top_left = top_left * factor;
		v_col.top_right = top_right * factor;
		
		return v_col;
	}
}

[System.Serializable]
public class ActionVariableProgression
{
	public ValueProgression m_progression = ValueProgression.Constant;
	public EasingEquation m_ease_type = EasingEquation.Linear;
	public bool m_is_offset_from_last = false;
	public bool m_to_to_bool = false;
	public bool m_unique_randoms = false;
}

[System.Serializable]
public class ActionFloatProgression : ActionVariableProgression
{
	public float m_value = 0;
	public float m_from = 0;
	public float m_to = 0;
	public float m_to_to = 0;
	
	public int NumEditorLines
	{
		get
		{
			if(m_progression == ValueProgression.Constant)
			{
				return 2;
			}
			else if(m_progression == ValueProgression.Random || (m_progression == ValueProgression.Eased && !m_to_to_bool))
			{
				return 3;
			}
			else
			{
				return 4;
			}
		}
	}
	
	public ActionFloatProgression(float start_val)
	{
		m_value = start_val;
		m_from = start_val;
		m_to = start_val;
		m_to_to = start_val;
	}
	
	public void CalculateProgression(float progression)
	{
		if(m_progression == ValueProgression.Random && (progression >= 0 || m_unique_randoms))
		{
			m_value = m_from + (m_to - m_from) * Random.value;
		}
		else if(m_progression == ValueProgression.Eased)
		{
			if(m_to_to_bool)
			{
				if(progression  <= 0.5f)
				{
					m_value = m_from + (m_to - m_from) * EffectManager.GetEaseProgress(m_ease_type, progression/0.5f);
				}
				else
				{
					progression -= 0.5f;
					m_value = m_to + (m_to_to - m_to) * EffectManager.GetEaseProgress(EffectManager.GetEaseTypeOpposite(m_ease_type), progression/0.5f);
				}
			}
			else
			{
				m_value = m_from + (m_to - m_from) * EffectManager.GetEaseProgress(m_ease_type, progression);
			}
		}
		else if(m_progression == ValueProgression.Constant)
		{
			m_value = m_from;
		}
	}
	
	public ActionFloatProgression Clone()
	{
		ActionFloatProgression float_progression = new ActionFloatProgression(0);
		
		float_progression.m_progression = m_progression;
		float_progression.m_ease_type = m_ease_type;
		float_progression.m_from = m_from;
		float_progression.m_to = m_to;
		float_progression.m_to_to = m_to_to;
		float_progression.m_to_to_bool = m_to_to_bool;
		float_progression.m_unique_randoms = m_unique_randoms;
		
		return float_progression;
	}
}

[System.Serializable]
public class ActionPositionVector3Progression : ActionVector3Progression
{
	public bool m_force_position_override = false;
	
	public override int NumEditorLines
	{
		get
		{
			if(m_progression == ValueProgression.Constant)
			{
				return 4;
			}
			else if(m_progression == ValueProgression.Random || (m_progression == ValueProgression.Eased && !m_to_to_bool))
			{
				return 6;
			}
			else
			{
				return 8;
			}
		}
	}
	
	public ActionPositionVector3Progression(Vector3 start_vec)
	{
		m_value = start_vec;
		m_from = start_vec;
		m_to = start_vec;
		m_to_to = start_vec;
	}
	
	public ActionPositionVector3Progression CloneThis()
	{
		ActionPositionVector3Progression progression = new ActionPositionVector3Progression(Vector3.zero);
		
		progression.m_progression = m_progression;
		progression.m_ease_type = m_ease_type;
		progression.m_from = m_from;
		progression.m_to = m_to;
		progression.m_to_to = m_to_to;
		progression.m_to_to_bool = m_to_to_bool;
		progression.m_is_offset_from_last = m_is_offset_from_last;
		progression.m_unique_randoms = m_unique_randoms;
		progression.m_force_position_override = m_force_position_override;
		
		return progression;
	}
}

[System.Serializable]
public class ActionVector3Progression : ActionVariableProgression
{
	public Vector3 m_value = Vector3.zero;
	public Vector3 m_from = Vector3.zero;
	public Vector3 m_to = Vector3.zero;
	public Vector3 m_to_to = Vector3.zero;
	
	public virtual int NumEditorLines
	{
		get
		{
			if(m_progression == ValueProgression.Constant)
			{
				return 3;
			}
			else if(m_progression == ValueProgression.Random || (m_progression == ValueProgression.Eased && !m_to_to_bool))
			{
				return 5;
			}
			else
			{
				return 7;
			}
		}
	}
	
	public ActionVector3Progression()
	{
		
	}
	
	public ActionVector3Progression(Vector3 start_vec)
	{
		m_value = start_vec;
		m_from = start_vec;
		m_to = start_vec;
		m_to_to = start_vec;
	}
	
	public void CalculateProgression(float progression, Vector3 offset_vec)
	{
		if(m_progression != ValueProgression.Random || progression >= 0 || m_unique_randoms)
		{
			m_value = m_is_offset_from_last ? offset_vec : Vector3.zero;
		}
		
		if(m_progression == ValueProgression.Random && (progression >= 0 || m_unique_randoms))
		{
			m_value += new Vector3(m_from.x + (m_to.x - m_from.x) * Random.value, m_from.y + (m_to.y - m_from.y) * Random.value, m_from.z + (m_to.z - m_from.z) * Random.value);
		}
		else if(m_progression == ValueProgression.Eased)
		{
			if(m_to_to_bool)
			{
				if(progression  <= 0.5f)
				{
					m_value += m_from + (m_to - m_from) * EffectManager.GetEaseProgress(m_ease_type, progression/0.5f);
				}
				else
				{
					progression -= 0.5f;
					m_value += m_to + (m_to_to - m_to) * EffectManager.GetEaseProgress(EffectManager.GetEaseTypeOpposite(m_ease_type), progression/0.5f);
				}
			}
			else
			{
				m_value += m_from + (m_to - m_from) * EffectManager.GetEaseProgress(m_ease_type, progression);
			}
		}
		else if(m_progression == ValueProgression.Constant)
		{
			m_value += m_from;
		}
	}
	
	public ActionVector3Progression Clone()
	{
		ActionVector3Progression vector3_progression = new ActionVector3Progression(Vector3.zero);
		
		vector3_progression.m_progression = m_progression;
		vector3_progression.m_ease_type = m_ease_type;
		vector3_progression.m_from = m_from;
		vector3_progression.m_to = m_to;
		vector3_progression.m_to_to = m_to_to;
		vector3_progression.m_to_to_bool = m_to_to_bool;
		vector3_progression.m_is_offset_from_last = m_is_offset_from_last;
		vector3_progression.m_unique_randoms = m_unique_randoms;
		
		return vector3_progression;
	}
}


[System.Serializable]
public class ActionColorProgression : ActionVariableProgression
{
	public Color m_value = Color.white;
	public Color m_from = Color.white;
	public Color m_to = Color.white;
	public Color m_to_to = Color.white;
	
	public int NumEditorLines
	{
		get
		{
			return 2;
		}
	}
	
	public ActionColorProgression(Color start_colour)
	{
		m_value = start_colour;
		m_from = start_colour;
		m_to = start_colour;
		m_to_to = start_colour;
	}
	
	public void CalculateProgression(float progression, Color offset_col)
	{
		m_value = m_is_offset_from_last ? offset_col : new Color(0,0,0,0);
		
		if(m_progression == ValueProgression.Random && (progression >= 0 || m_unique_randoms))
		{
			m_value += m_from + (m_to - m_from) * Random.value;
		}
		else if(m_progression == ValueProgression.Eased)
		{
			if(m_to_to_bool)
			{
				if(progression  <= 0.5f)
				{
					m_value += m_from + (m_to - m_from) * EffectManager.GetEaseProgress(m_ease_type, progression/0.5f);
				}
				else
				{
					progression -= 0.5f;
					m_value += m_to + (m_to_to - m_to) * EffectManager.GetEaseProgress(EffectManager.GetEaseTypeOpposite(m_ease_type), progression/0.5f);
				}
			}
			else
			{
				m_value += m_from + (m_to - m_from) * EffectManager.GetEaseProgress(m_ease_type, progression);
			}
		}
		else if(m_progression == ValueProgression.Constant)
		{
			m_value += m_from;
		}
	}
	
	public ActionColorProgression Clone()
	{
		ActionColorProgression color_progression = new ActionColorProgression(Color.white);
		
		color_progression.m_progression = m_progression;
		color_progression.m_ease_type = m_ease_type;
		color_progression.m_from = m_from;
		color_progression.m_to = m_to;
		color_progression.m_to_to = m_to_to;
		color_progression.m_to_to_bool = m_to_to_bool;
		color_progression.m_is_offset_from_last = m_is_offset_from_last;
		color_progression.m_unique_randoms = m_unique_randoms;
		
		return color_progression;
	}
}

[System.Serializable]
public class ActionVertexColorProgression : ActionVariableProgression
{
	public VertexColour m_value = new VertexColour();
	public VertexColour m_from = new VertexColour();
	public VertexColour m_to = new VertexColour();
	public VertexColour m_to_to = new VertexColour();
	
	public int NumEditorLines
	{
		get
		{
			return 3;
		}
	}
	
	public ActionVertexColorProgression(VertexColour start_colour)
	{
		m_value = start_colour.Clone();
		m_from = start_colour.Clone();
		m_to = start_colour.Clone();
		m_to_to = start_colour.Clone();
	}
	
	public void CalculateProgression(float progression, VertexColour offset_colour)
	{
		m_value = m_is_offset_from_last ? offset_colour.Clone() : new VertexColour(new Color(0,0,0,0));
		
		if(m_progression == ValueProgression.Random && (progression >= 0 || m_unique_randoms))
		{
			m_value = m_value.Add(m_from.Add(m_to.Sub(m_from).Multiply(Random.value)));
		}
		else if(m_progression == ValueProgression.Eased)
		{
			if(m_to_to_bool)
			{
				if(progression  <= 0.5f)
				{
					m_value = m_value.Add(m_from.Add((m_to.Sub(m_from)).Multiply(EffectManager.GetEaseProgress(m_ease_type, progression/0.5f))));
				}
				else
				{
					progression -= 0.5f;
					m_value = m_value.Add(m_to.Add((m_to_to.Sub(m_to)).Multiply(EffectManager.GetEaseProgress(m_ease_type, progression/0.5f))));
				}
			}
			else
			{
				m_value = m_value.Add(m_from.Add((m_to.Sub(m_from)).Multiply(EffectManager.GetEaseProgress(m_ease_type, progression))));
			}
		}
		else if(m_progression == ValueProgression.Constant)
		{
			m_value = m_value.Add(m_from);
		}
	}
	
	public ActionVertexColorProgression Clone()
	{
		ActionVertexColorProgression color_progression = new ActionVertexColorProgression(new VertexColour());
		
		color_progression.m_progression = m_progression;
		color_progression.m_ease_type = m_ease_type;
		color_progression.m_from = m_from.Clone();
		color_progression.m_to = m_to.Clone();
		color_progression.m_to_to = m_to_to.Clone();
		color_progression.m_to_to_bool = m_to_to_bool;
		color_progression.m_is_offset_from_last = m_is_offset_from_last;
		color_progression.m_unique_randoms = m_unique_randoms;
		
		return color_progression;
	}
}

[System.Serializable]
public class LetterAction
{
	bool m_editor_folded = false;
	public bool FoldedInEditor { get { return m_editor_folded; } set { m_editor_folded = value; } }
	
	public bool m_offset_from_last = false;
	public bool m_use_gradient = false;
	
	public VertexColour m_start_colour_value;
	public ActionColorProgression m_start_colour = new ActionColorProgression(Color.white);
	public ActionColorProgression m_end_colour = new ActionColorProgression(Color.white);
	public VertexColour m_end_colour_value;
	public ActionVertexColorProgression m_start_vertex_colour = new ActionVertexColorProgression(new VertexColour());
	public ActionVertexColorProgression m_end_vertex_colour = new ActionVertexColorProgression(new VertexColour());
	
	public ActionPositionVector3Progression m_start_pos = new ActionPositionVector3Progression(Vector3.zero);
	public ActionPositionVector3Progression m_end_pos = new ActionPositionVector3Progression(Vector3.zero);
	
	public ActionVector3Progression m_start_euler_rotation = new ActionVector3Progression(Vector3.zero);
	public ActionVector3Progression m_end_euler_rotation = new ActionVector3Progression(Vector3.zero);
	public ActionVector3Progression m_start_scale = new ActionVector3Progression(Vector3.one);
	public ActionVector3Progression m_end_scale = new ActionVector3Progression(Vector3.one);
	
	public bool m_force_same_start_time = false;
	
	public ActionFloatProgression m_delay_progression = new ActionFloatProgression(0);
	public ActionFloatProgression m_duration_progression = new ActionFloatProgression(1);
	
	public EasingEquation m_ease_type = EasingEquation.Linear;
	public TextAnimLoopType m_loop_type = TextAnimLoopType.NONE;
	public int m_repeat_times = 0;
	
	public TextAnchor m_letter_anchor = TextAnchor.MiddleCenter;
	
	public void SoftReset(LetterAction prev_action)
	{
		if(!m_use_gradient)
		{
			if(!m_offset_from_last && m_start_colour.m_progression == ValueProgression.Random)
			{
				m_start_colour.CalculateProgression(-1, prev_action != null ? prev_action.m_end_colour.m_value : Color.black);
				m_start_colour_value = new VertexColour(m_start_colour.m_value);
			}
		}
		else
		{
			if(!m_offset_from_last && m_start_vertex_colour.m_progression == ValueProgression.Random)
			{
				m_start_vertex_colour.CalculateProgression(-1, prev_action != null ? prev_action.m_end_vertex_colour.m_value.Clone() : new VertexColour(Color.black));
				m_start_colour_value = new VertexColour(m_start_vertex_colour.m_value);
			}
		}
		
		if(!m_offset_from_last)
		{
			if(m_start_pos.m_progression == ValueProgression.Random)
			{
				m_start_pos.CalculateProgression(-1, prev_action != null ? prev_action.m_end_pos.m_value : Vector3.zero);
			}
			if(m_start_euler_rotation.m_progression == ValueProgression.Random)
			{
				m_start_euler_rotation.CalculateProgression(-1, prev_action != null ? prev_action.m_end_euler_rotation.m_value : Vector3.zero);
			}
			if(m_start_scale.m_progression == ValueProgression.Random)
			{
				m_start_scale.CalculateProgression(-1, prev_action != null ? prev_action.m_end_scale.m_value : Vector3.zero);
			}
		}
		
		if(m_delay_progression.m_progression == ValueProgression.Random)
		{
			m_delay_progression.CalculateProgression(-1);
		}
		if(m_duration_progression.m_progression == ValueProgression.Random)
		{
			m_duration_progression.CalculateProgression(-1);
		}
	}
	
	public void Reset(int action_idx, float letter_progression, LetterAction prev_action)
	{
		if(!m_use_gradient)
		{
			if(m_offset_from_last && prev_action != null)
			{
				m_start_colour_value = prev_action.m_end_colour_value.Clone();
			}
			else
			{
				m_start_colour.CalculateProgression(letter_progression, prev_action != null ? prev_action.m_end_colour.m_value : Color.black);
				m_start_colour_value = new VertexColour(m_start_colour.m_value);
			}
			m_end_colour.CalculateProgression(letter_progression, m_start_colour_value.top_left);
			m_end_colour_value = new VertexColour(m_end_colour.m_value);
		}
		else
		{
			if(m_offset_from_last && prev_action != null)
			{
				m_start_colour_value = prev_action.m_end_colour_value.Clone();
			}
			else
			{
				m_start_vertex_colour.CalculateProgression(letter_progression, prev_action != null ? prev_action.m_end_vertex_colour.m_value.Clone() : new VertexColour(Color.black));
				m_start_colour_value = new VertexColour(m_start_vertex_colour.m_value);
			}
			m_end_vertex_colour.CalculateProgression(letter_progression, m_start_colour_value.Clone());
			m_end_colour_value = new VertexColour(m_end_vertex_colour.m_value);
		}
		
		if(m_offset_from_last && prev_action != null)
		{
			m_start_pos.m_value = prev_action.m_end_pos.m_value;
			m_start_euler_rotation.m_value = prev_action.m_end_euler_rotation.m_value;
			m_start_scale.m_value = prev_action.m_end_scale.m_value;
		}
		else
		{
			m_start_pos.CalculateProgression(letter_progression, prev_action != null ? prev_action.m_end_pos.m_value : Vector3.zero);
			m_start_euler_rotation.CalculateProgression(letter_progression, prev_action != null ? prev_action.m_end_euler_rotation.m_value : Vector3.zero);
			m_start_scale.CalculateProgression(letter_progression, prev_action != null ? prev_action.m_end_scale.m_value : Vector3.zero);
		}
		
		m_end_pos.CalculateProgression(letter_progression, m_start_pos.m_value);
		m_end_euler_rotation.CalculateProgression(letter_progression, m_start_euler_rotation.m_value);
		
		m_end_scale.CalculateProgression(letter_progression, m_start_scale.m_value);
		
		m_delay_progression.CalculateProgression(letter_progression);
		m_duration_progression.CalculateProgression(letter_progression);
		
		if(letter_progression == 0)
		{
			if(m_delay_progression.m_value < 0)
			{
				Debug.LogWarning("Delay value on Action " + action_idx + " is less than zero. Delay = " + m_delay_progression.m_value );
			}
			if(m_duration_progression.m_value <= 0)
			{
				Debug.LogWarning("Duration value on Action " + action_idx + " is less than or equal to zero. Duration = " + m_duration_progression.m_value );
			}
		}
	}
	
	public LetterAction Clone(LetterAction last_action, bool continue_from_last, int total_letters, int letter_idx = -1)
	{
		LetterAction letter_action = new LetterAction();
		
		letter_action.m_use_gradient = m_use_gradient;
		
		letter_action.m_start_colour = continue_from_last ? last_action.m_end_colour.Clone() : m_start_colour.Clone();
		letter_action.m_end_colour = m_end_colour.Clone();
		
		letter_action.m_start_vertex_colour = continue_from_last ? last_action.m_end_vertex_colour.Clone() : m_start_vertex_colour.Clone();
		letter_action.m_end_vertex_colour = m_end_vertex_colour.Clone();
		
		letter_action.m_start_pos = continue_from_last ? last_action.m_end_pos.CloneThis() : m_start_pos.CloneThis();
		letter_action.m_end_pos = m_end_pos.CloneThis();
		
		letter_action.m_start_euler_rotation = continue_from_last ? last_action.m_end_euler_rotation.Clone() : m_start_euler_rotation.Clone();
		letter_action.m_end_euler_rotation = m_end_euler_rotation.Clone();
		
		letter_action.m_start_scale = continue_from_last ? last_action.m_end_scale.Clone() : m_start_scale.Clone();
		letter_action.m_end_scale = m_end_scale.Clone();
		
		letter_action.m_delay_progression = m_delay_progression.Clone();
		letter_action.m_duration_progression = m_duration_progression.Clone();
		
		letter_action.m_loop_type = m_loop_type;
		letter_action.m_repeat_times = m_repeat_times;
		
		letter_action.m_letter_anchor = m_letter_anchor;
		letter_action.m_ease_type = m_ease_type;
		
		letter_action.m_offset_from_last = m_offset_from_last;
		letter_action.m_force_same_start_time = m_force_same_start_time;
		
		return letter_action;
	}
	
	public LetterAction ContinueActionFromThis()
	{
		LetterAction letter_action = new LetterAction();
		
		// Default to offset from previous and not be folded in editor
		letter_action.m_offset_from_last = true;
		letter_action.m_editor_folded = true;
		
		letter_action.m_use_gradient = m_use_gradient;
		
		letter_action.m_start_colour = m_end_colour.Clone();
		letter_action.m_end_colour = m_end_colour.Clone();
		letter_action.m_start_vertex_colour = m_end_vertex_colour.Clone();
		letter_action.m_end_vertex_colour = m_end_vertex_colour.Clone();
		
		letter_action.m_start_pos = m_end_pos.CloneThis();
		letter_action.m_end_pos = m_end_pos.CloneThis();
		
		letter_action.m_start_euler_rotation = m_end_euler_rotation.Clone();
		letter_action.m_end_euler_rotation = m_end_euler_rotation.Clone();
		
		letter_action.m_start_scale = m_end_scale.Clone();
		letter_action.m_end_scale = m_end_scale.Clone();
		
		letter_action.m_delay_progression = new ActionFloatProgression(0);
		letter_action.m_duration_progression = new ActionFloatProgression(1);
		
		letter_action.m_loop_type = m_loop_type;
		letter_action.m_repeat_times = m_repeat_times;
		
		letter_action.m_letter_anchor = m_letter_anchor;
		letter_action.m_ease_type = m_ease_type;
		
		return letter_action;
	}
}

public enum LOOP_TYPE
{
	LOOP,
	LOOP_REVERSE
}

[System.Serializable]
public class ActionLoopCycle
{
	public int m_start_action_idx = 0;
	public int m_end_action_idx = 0;
	public int m_number_of_loops = 0;
	public LOOP_TYPE m_loop_type = LOOP_TYPE.LOOP;
	
	public ActionLoopCycle(int start, int end)
	{
		m_start_action_idx = start;
		m_end_action_idx = end;
	}
	
	public ActionLoopCycle Clone()
	{
		ActionLoopCycle action_loop = new ActionLoopCycle(m_start_action_idx,m_end_action_idx);
		
		action_loop.m_number_of_loops = m_number_of_loops;
		action_loop.m_loop_type = m_loop_type;
		
		return action_loop;
	}
	
	public int SpanWidth
	{
		get
		{
			return m_end_action_idx - m_start_action_idx;
		}
	}
}


[System.Serializable]
public class LetterAnimation
{
	public List<LetterAction> m_letter_actions = new List<LetterAction>();
	public List<ActionLoopCycle> m_loop_cycles = new List<ActionLoopCycle>();
	[HideInInspector]
	public List<ActionLoopCycle> m_active_loop_cycles;
	
	int m_action_index = 0;
	public int ActionIndex { get { return m_action_index; } }
	
	[HideInInspector]
	public bool m_reverse = false;
	
	int m_action_progress = 0;
	public int ActionProgress { get { return m_action_progress; } }
	
	bool m_active = false;
	public bool Active { get { return m_active; } set { m_active = value; } }
	
	public LetterAnimation()
	{
		m_letter_actions = new List<LetterAction>();
	}
	
	public LetterAnimation Clone(int letter_idx, int total_letters)
	{
		LetterAnimation letter_anim = new LetterAnimation();
		LetterAction last_action = null;
		foreach(LetterAction action in m_letter_actions)
		{
			letter_anim.m_letter_actions.Add(action.Clone(last_action != null ? last_action : null, last_action != null && action.m_offset_from_last, total_letters, letter_idx));
			
			last_action = action;
		}
		
		letter_anim.m_loop_cycles = new List<ActionLoopCycle>();
		foreach(ActionLoopCycle loop_cycle in m_loop_cycles)
		{
			letter_anim.m_loop_cycles.Add(loop_cycle.Clone());
		}
		
		return letter_anim;
	}
	
	public void Reset(float letter_progression)
	{
		m_action_index = 0;
		m_action_progress = 0;
		m_active_loop_cycles = new List<ActionLoopCycle>();
		
		if(m_loop_cycles.Count > 0)
		{
			UpdateLoopList();
		}
		
		int num_actions = m_letter_actions.Count;
		for(int action_idx=0; action_idx < num_actions; action_idx++)
		{
			m_letter_actions[action_idx].Reset(action_idx, letter_progression, action_idx > 0 ? m_letter_actions[action_idx-1] : null);
		}
	}
	
	public void SetNextActionIndex()
	{
		// based on current active loop list, return the next action index
		
		// increment action progress count
		m_action_progress++;
		
		ActionLoopCycle current_loop;
		for(int loop_idx=0; loop_idx < m_active_loop_cycles.Count; loop_idx++)
		{
			current_loop = m_active_loop_cycles[loop_idx];
			
			if((current_loop.m_loop_type == LOOP_TYPE.LOOP && m_action_index == current_loop.m_end_action_idx) ||
				(current_loop.m_loop_type == LOOP_TYPE.LOOP_REVERSE && 
					((m_reverse && m_action_index == current_loop.m_start_action_idx) || (!m_reverse && m_action_index == current_loop.m_end_action_idx))
				))
			{
				
				// Reached end of loop cycle. Deduct one cycle from loop count.
				bool end_of_loop_cycle = current_loop.m_loop_type == LOOP_TYPE.LOOP || m_reverse;
				
				if(end_of_loop_cycle)
				{
					current_loop.m_number_of_loops--;
				}
				
				// Switch reverse status
				if(current_loop.m_loop_type == LOOP_TYPE.LOOP_REVERSE)
				{
					m_reverse = !m_reverse;
				}
				
				if(end_of_loop_cycle && current_loop.m_number_of_loops == 0)
				{
					// loop cycle finished
					// Remove this loop from active loop list
					m_active_loop_cycles.RemoveAt(loop_idx);
					loop_idx--;
				}
				else
				{
					if(current_loop.m_number_of_loops < 0)
					{
						current_loop.m_number_of_loops = -1;
					}
					
					// return to the start of this loop again
					if(current_loop.m_loop_type == LOOP_TYPE.LOOP)
					{
						m_action_index = current_loop.m_start_action_idx;
					}
					
					return;
				}
				
				
			}
			else
			{
				break;
			}
		}
		
		m_action_index += (m_reverse ? -1 : 1);
		
		// check for animation reaching end
		if(m_action_index >= m_letter_actions.Count)
		{
			m_active = false;
			m_action_index = m_letter_actions.Count -1;
		}
		
		return;
	}
	
	// Only called if action_idx has changed since last time
	public void UpdateLoopList()
	{
		// add any new loops from the next action index to the loop list
		
		foreach(ActionLoopCycle loop in m_loop_cycles)
		{
			if(loop.m_start_action_idx == m_action_index)
			{
				// add this new loop into the ordered active loop list
				int new_loop_cycle_span = loop.SpanWidth;
				
				int loop_idx = 0;
				foreach(ActionLoopCycle active_loop in m_active_loop_cycles)
				{
					if(loop.m_start_action_idx == active_loop.m_start_action_idx && loop.m_end_action_idx == active_loop.m_end_action_idx)
					{
						// This loop is already in the active loop list, don't re-add
						loop_idx = -1;
						break;
					}
					
					if(new_loop_cycle_span < active_loop.SpanWidth)
					{
						break;
					}
						
					loop_idx++;
				}
				
				if(loop_idx >= 0)
				{
					m_active_loop_cycles.Insert(loop_idx, loop.Clone());
				}
			}
		}
		
		
	}
	
	public void AddLoop(int start_idx, int end_idx, bool change_type)
	{
		bool valid_loop_addition = true;
		int insert_at_idx = 0;
		
		if(end_idx >= start_idx && start_idx >= 0 && start_idx < m_letter_actions.Count && end_idx >= 0 && end_idx < m_letter_actions.Count)
		{
			int new_loop_width = end_idx - start_idx;
			int count = 1;
			foreach(ActionLoopCycle loop in m_loop_cycles)
			{
				if((start_idx < loop.m_start_action_idx && (end_idx >loop.m_start_action_idx && end_idx < loop.m_end_action_idx))
					|| (end_idx > loop.m_end_action_idx && (start_idx > loop.m_start_action_idx && start_idx < loop.m_end_action_idx)))
				{
					// invalid loop
					valid_loop_addition = false;
					Debug.LogWarning("Invalid Loop Added: Loops can not intersect other loops.");
					break;
				}
				else if(start_idx == loop.m_start_action_idx && end_idx == loop.m_end_action_idx)
				{
					// Entry already exists, so either add to it, or change its type
					valid_loop_addition = false;
					if(change_type)
					{
						loop.m_loop_type = loop.m_loop_type == LOOP_TYPE.LOOP ? LOOP_TYPE.LOOP_REVERSE : LOOP_TYPE.LOOP;
					}
					else
					{
						loop.m_number_of_loops ++;
					}
					break;
				}
				else
				{
					if(new_loop_width >= loop.SpanWidth)
					{
						insert_at_idx = count;
					}
				}
						
				count++;
			}
		}
		else
		{
			valid_loop_addition = false;
			Debug.LogWarning("Invalid Loop Added: Check that start/end index are in bounds.");
		}
		
		
		if(valid_loop_addition)
		{
			m_loop_cycles.Insert(insert_at_idx, new ActionLoopCycle(start_idx, end_idx));
		}
	}
}

public enum ValueProgression
{
	Constant,
	Random,
	Eased
}

public enum TextDisplayAxis
{
	HORIZONTAL,
	VERTICAL
}

public enum EasingEquation
{
    Linear,
    QuadEaseOut, QuadEaseIn, QuadEaseInOut, QuadEaseOutIn,
    ExpoEaseOut, ExpoEaseIn, ExpoEaseInOut, ExpoEaseOutIn,
    CubicEaseOut, CubicEaseIn, CubicEaseInOut, CubicEaseOutIn,
    QuartEaseOut, QuartEaseIn, QuartEaseInOut, QuartEaseOutIn,
    QuintEaseOut, QuintEaseIn, QuintEaseInOut, QuintEaseOutIn,
    CircEaseOut, CircEaseIn, CircEaseInOut, CircEaseOutIn,
    SineEaseOut, SineEaseIn, SineEaseInOut, SineEaseOutIn,
    ElasticEaseOut, ElasticEaseIn, ElasticEaseInOut, ElasticEaseOutIn,
    BounceEaseOut, BounceEaseIn, BounceEaseInOut, BounceEaseOutIn,
    BackEaseOut, BackEaseIn, BackEaseInOut, BackEaseOutIn
}

public class TextSizeData
{
	public float x_min;
	public float x_max;
	public float y_min;
	public float y_max;
	public float text_width;
	public float text_height;
}

[System.Serializable]
public class LetterSetup
{
	public string m_character;
	public int m_letter_idx = -1;
	public int m_text_length = 0;
	public bool m_flipped = false;
	public float m_width = 0;
	public float m_height = 0;
	public Vector3[] m_base_vertices;
	public Vector3 m_base_offset;
	public Mesh m_mesh;
	public float m_x_offset = 0;
	public float m_y_offset = 0;
	
	public bool m_waiting_to_sync = false;
	
	public LetterAnimation m_letter_animation;
	float m_timer_offset = 0;
	
	public LetterSetup(string character, int letter_idx, int text_length, Mesh mesh, Vector3 base_offset, CustomCharacterInfo char_info, LetterAnimation animation, TextAnchor text_anchor, TextDisplayAxis display_axis, TextSizeData text_data)
	{
		m_character = character;
		m_letter_idx = letter_idx;
		m_text_length = text_length;
		m_mesh = mesh;
		m_base_offset = base_offset;
		
		SetupLetterMesh(char_info);
		SetupBaseOffsets(text_anchor, display_axis, text_data);
		
		if(m_flipped)
		{
			// flip UV coords in x axis.
			m_mesh.uv = new Vector2[] {mesh.uv[3], mesh.uv[2], mesh.uv[1], mesh.uv[0]};
		}
		
		if(animation != null)
		{
			m_letter_animation = animation;
		}
		else
		{
			m_letter_animation = new LetterAnimation();
		}
	}
	
	public void SetupLetterMesh(CustomCharacterInfo char_info)
	{
		m_width = char_info.vert.width;
		m_height = char_info.vert.height;
		m_flipped = char_info.flipped;
		
		// Setup base vertices
		m_x_offset = char_info.vert.x;
		m_y_offset = char_info.vert.y;
		
		if(!m_flipped)
		{
			m_base_vertices = new Vector3[] { new Vector3(m_width, 0, 0), new Vector3(0, 0, 0), new Vector3(0, m_height, 0), new Vector3(m_width, m_height, 0)};
		}
		else
		{
			// rotate order of vertices by one.
			m_base_vertices = new Vector3[] {new Vector3(0, 0, 0), new Vector3(0, m_height, 0), new Vector3( m_width, m_height, 0), new Vector3(m_width, 0, 0)};
		}
	}
	
	public void Reset(float letter_progression)
	{
		m_letter_animation.Active = true;
		m_timer_offset = 0;
		m_letter_animation.Reset(letter_progression);
	}
	
	public void SetupBaseOffsets(TextAnchor anchor, TextDisplayAxis display_axis, TextSizeData text_data)
	{
		if(display_axis != TextDisplayAxis.VERTICAL)
		{
			m_base_offset += new Vector3(m_x_offset, m_y_offset, 0);
		}
		
		m_base_offset.y -= text_data.y_max;
		
		// Handle text y offset
		if(anchor == TextAnchor.MiddleLeft || anchor == TextAnchor.MiddleCenter || anchor == TextAnchor.MiddleRight)
		{
			m_base_offset.y += text_data.text_height / 2;
		}
		else if(anchor == TextAnchor.LowerLeft || anchor == TextAnchor.LowerCenter || anchor == TextAnchor.LowerRight)
		{
			m_base_offset.y += text_data.text_height;
		}
		
		// Handle text x offset
		if(anchor == TextAnchor.LowerRight || anchor == TextAnchor.MiddleRight || anchor == TextAnchor.UpperRight)
		{
			m_base_offset.x -= text_data.text_width;
		}
		else if(anchor == TextAnchor.LowerCenter || anchor == TextAnchor.MiddleCenter || anchor == TextAnchor.UpperCenter)
		{
			m_base_offset.x -= (text_data.text_width/2);
		}
	}
	
	public void SetMeshState(int action_idx, float action_progress)
	{
		if(action_idx < m_letter_animation.m_letter_actions.Count)
		{
			SetupMesh(m_letter_animation.m_letter_actions[action_idx], Mathf.Clamp(action_progress, 0,1));
		}
		else
		{
			// action not found for this letter. Position letter in its default position
			
			Vector3[] mesh_verts = new Vector3[4];
			for(int idx=0; idx < 4; idx++)
			{
				mesh_verts[idx] = m_base_vertices[idx] + m_base_offset;
			}
			m_mesh.vertices = mesh_verts;
		}
	}
	
	// Animates the letter mesh and return the current action index in use
	public bool AnimateMesh(float timer, TextAnchor text_anchor, int lowest_action_progress)
	{
		LetterAnimation animation = null;
		LetterAction letter_action = null;
		
		if(m_letter_animation.m_letter_actions.Count > 0)
		{
			if(!m_letter_animation.Active)
			{
				return false;
			}
			
			animation = m_letter_animation;
			letter_action = m_letter_animation.m_letter_actions[m_letter_animation.ActionIndex];
		}
		
		if(letter_action != null)
		{
			if(m_waiting_to_sync)
			{
				if(lowest_action_progress < animation.ActionProgress)
				{
					return true;
				}
				else
				{
					m_waiting_to_sync = false;
					
					// reset timer offset to compensate for the sync-up wait time
					m_timer_offset = timer;
				}
			}
			else if(!animation.m_reverse && letter_action.m_force_same_start_time && lowest_action_progress < animation.ActionProgress)
			{
				// Force letter to wait for rest of letters to be in sync
				m_waiting_to_sync = true;
				
				return true;
			}
			
			float action_progress = 0;
			float action_delay = letter_action.m_delay_progression.m_value >= 0 ? letter_action.m_delay_progression.m_value : 0;
			float action_duration = letter_action.m_duration_progression.m_value >= 0 ? letter_action.m_duration_progression.m_value : 0;
			float action_timer = timer - m_timer_offset;
			
			if((animation.m_reverse || action_timer > action_delay))
			{
				float linear_progress = (action_timer - (animation.m_reverse ? 0 : action_delay)) / action_duration;
				
				if(animation.m_reverse)
				{
					if(action_timer >= action_duration)
					{
						linear_progress = 0;
					}
					else
					{
						linear_progress = 1 - linear_progress;
					}
				}
				
				action_progress = EffectManager.GetEaseProgress(letter_action.m_ease_type, linear_progress);
				
				if((!animation.m_reverse && linear_progress >= 1) || (animation.m_reverse && action_timer >= action_duration + action_delay))
				{
					action_progress = animation.m_reverse ? 0 : 1;
					
					int prev_action_idx = animation.ActionIndex;
					
					// Set next action index
					animation.SetNextActionIndex();
					
					if(!animation.m_reverse && animation.ActionProgress > animation.ActionIndex)
					{
						// Repeating the action again; check for unqiue random variable requests.
						animation.m_letter_actions[animation.ActionIndex].SoftReset(animation.m_letter_actions[prev_action_idx]);
					}
					
					if(animation.Active)
					{
						// Add to the timer offset
						m_timer_offset += action_delay + action_duration;
						
						if(prev_action_idx != animation.ActionIndex)
						{
							animation.UpdateLoopList();
						}
					}
					
				}
			}
			
			SetupMesh(letter_action, action_progress);
		}
		else
		{
			// no actions found for this letter. Position letter in its default position
			Vector3[] mesh_verts = new Vector3[4];
			for(int idx=0; idx < 4; idx++)
			{
				mesh_verts[idx] = m_base_vertices[idx] + m_base_offset;
			}
			m_mesh.vertices = mesh_verts;
		}
		
		if(animation != null)
		{
			return animation.Active;
		}
		
		return false;
	}
	
	void SetupMesh(LetterAction letter_action, float action_progress)
	{
		// TODO: Check if anything has changed? Might just be colour changing, therefore this doesn't need doing!
		Vector3[] mesh_verts = new Vector3[4];
		for(int idx=0; idx < 4; idx++)
		{
			// scale vertices
			Vector3 current_scale = EffectManager.Vector3Lerp(letter_action.m_start_scale.m_value, letter_action.m_end_scale.m_value, action_progress);
			
			// rotate vertices
			// handle letter anchor x-offset
			Vector3 rotation_offset = Vector3.zero;
			if(letter_action.m_letter_anchor == TextAnchor.UpperRight || letter_action.m_letter_anchor == TextAnchor.MiddleRight || letter_action.m_letter_anchor == TextAnchor.LowerRight)
			{
				rotation_offset += new Vector3(m_width, 0, 0);
			}
			else if(letter_action.m_letter_anchor == TextAnchor.UpperCenter || letter_action.m_letter_anchor == TextAnchor.MiddleCenter || letter_action.m_letter_anchor == TextAnchor.LowerCenter)
			{
				rotation_offset += new Vector3(m_width / 2, 0, 0);
			}
			
			// handle letter anchor y-offset
			if(letter_action.m_letter_anchor == TextAnchor.MiddleLeft || letter_action.m_letter_anchor == TextAnchor.MiddleCenter || letter_action.m_letter_anchor == TextAnchor.MiddleRight)
			{
				rotation_offset += new Vector3(0, m_height / 2, 0);
			}
			else if(letter_action.m_letter_anchor == TextAnchor.LowerLeft || letter_action.m_letter_anchor == TextAnchor.LowerCenter || letter_action.m_letter_anchor == TextAnchor.LowerRight)
			{
				rotation_offset += new Vector3(0, m_height, 0);
			}
			
			mesh_verts[idx] = m_base_vertices[idx];
			
			mesh_verts[idx] -= rotation_offset;
			mesh_verts[idx] = Vector3.Scale(mesh_verts[idx], current_scale);
			mesh_verts[idx] = Quaternion.Euler(EffectManager.Vector3Lerp(letter_action.m_start_euler_rotation.m_value, letter_action.m_end_euler_rotation.m_value, action_progress)) * mesh_verts[idx];
			mesh_verts[idx] += rotation_offset;
			
			// translate vertices
			mesh_verts[idx] += EffectManager.Vector3Lerp((!letter_action.m_start_pos.m_force_position_override ? m_base_offset : Vector3.zero) + letter_action.m_start_pos.m_value, (!letter_action.m_end_pos.m_force_position_override ? m_base_offset : Vector3.zero) + letter_action.m_end_pos.m_value, action_progress);
		}
		m_mesh.vertices = mesh_verts;
		
		// TODO: Needed?
		m_mesh.RecalculateNormals();
		
		// TODO: Check if this needs to be done first
//		if(letter_action.m_use_gradient)
//		{
			if(!m_flipped)
			{
				m_mesh.colors = new Color[]{ 
					Color.Lerp(letter_action.m_start_colour_value.top_right, letter_action.m_end_colour_value.top_right, action_progress), 
					Color.Lerp(letter_action.m_start_colour_value.top_left, letter_action.m_end_colour_value.top_left, action_progress), 
					Color.Lerp(letter_action.m_start_colour_value.bottom_left, letter_action.m_end_colour_value.bottom_left, action_progress), 
					Color.Lerp(letter_action.m_start_colour_value.bottom_right, letter_action.m_end_colour_value.bottom_right, action_progress)};
			}
			else
			{
				m_mesh.colors = new Color[]{
					Color.Lerp(letter_action.m_start_colour_value.top_left, letter_action.m_end_colour_value.top_left, action_progress),
					Color.Lerp(letter_action.m_start_colour_value.bottom_left, letter_action.m_end_colour_value.bottom_left, action_progress),
					Color.Lerp(letter_action.m_start_colour_value.bottom_right, letter_action.m_end_colour_value.bottom_right, action_progress),
					Color.Lerp(letter_action.m_start_colour_value.top_right, letter_action.m_end_colour_value.top_right, action_progress)
				};
			}
//		}
//		else
//		{
//			Color colour = Color.Lerp(letter_action.m_start_colour.m_value, letter_action.m_end_colour.m_value, action_progress);
//			m_mesh.colors = new Color[]{ colour, colour, colour, colour};
//		}
	}
}

public enum TextAnimLoopType
{
	NONE,
	LOOP,
	LOOP_REVERSE
}

public enum AnimationTime
{
	GAME_TIME,
	REAL_TIME
}

public class CustomCharacterInfo
{
	public bool flipped = false;
	public Rect uv = new Rect();
	public Rect vert = new Rect();
	public float width = 0;
	
	public CustomCharacterInfo ScaleClone(float scale)
	{
		CustomCharacterInfo char_info = new CustomCharacterInfo();
		
		char_info.flipped = flipped;
		char_info.uv = new Rect(uv);
		char_info.vert = new Rect(vert);
		char_info.width = width;
		
		// Scale char_info values
		char_info.vert.x /= scale;
		char_info.vert.y /= scale;
		char_info.vert.width /= scale;
		char_info.vert.height /= scale;
		char_info.width /= scale;
		
		return char_info;
	}
}

public class CustomFontCharacterData
{
	public Hashtable m_character_infos;
	
	public CustomFontCharacterData()
	{
		m_character_infos = new Hashtable();
	}
}

[RequireComponent (typeof (MeshFilter))]
[RequireComponent (typeof (MeshRenderer))]
public class EffectManager: MonoBehaviour
{
	
	const float FONT_SCALE_FACTOR = 10f;
	float FontScale { get { return FONT_SCALE_FACTOR / m_character_size; } }
	
#if UNITY_4_0	
	public Font m_font;
#endif
	public TextAsset m_font_data_file;
	public Material m_font_material;
	public string m_text = "";
	public Vector2 m_px_offset = new Vector2(0,0);
	public float m_character_size = 1;
	public TextDisplayAxis m_display_axis = TextDisplayAxis.HORIZONTAL;
	public TextAnchor m_text_anchor = TextAnchor.MiddleCenter;
	public AnimationTime m_time_type = AnimationTime.GAME_TIME;
	public bool m_begin_on_start = true;
	
	[HideInInspector]
	public LetterAnimation m_master_animation;
	
	[HideInInspector]
	public LetterSetup[] m_letters;
	
	Renderer m_renderer = null;
	MeshFilter m_mesh_filter = null;
	MeshFilter Mesh_Filter
	{
		get 
		{
			if(m_mesh_filter == null)
			{
				m_mesh_filter = this.GetComponent<MeshFilter>();
			}
			return m_mesh_filter;
		}
	}
	
	float m_last_time = 0;
	CustomFontCharacterData m_custom_font_data;
	string m_current_font_data_file_name = "";
	float m_animation_timer = 0;
	int m_lowest_action_progress = 0;
	bool m_running = false;
	bool m_paused = false;
	public bool Playing { get { return m_running && !m_paused; } }
	public bool Paused
	{
		get
		{
			return m_paused;
		}
		set
		{
			m_paused = value;
			
			if(!m_paused && m_time_type == AnimationTime.REAL_TIME)
			{
				m_last_time = Time.realtimeSinceStartup;
			}
		}
	}
	
	void Start()
	{
		if(m_begin_on_start)
		{
			PlayAnimation();
		}
	}
	
	public void PlayAnimation(float delay = 0)
	{
		int num_letters = m_letters.Length;
		int count = 0;
		
		foreach(LetterSetup letter in m_letters)
		{
			letter.m_waiting_to_sync = false;
			letter.m_letter_animation = m_master_animation.Clone(count, num_letters);
			letter.Reset(num_letters > 1 ? (float)count / ((float)num_letters - 1f) : 0);
			count ++;
		}
		
		m_lowest_action_progress = 0;
		m_animation_timer = 0;
		if(m_time_type == AnimationTime.REAL_TIME)
		{
			m_last_time = Time.realtimeSinceStartup;
		}
		
		if(delay > 0)
		{
			StartCoroutine(PlayAnimationAfterDelay(delay));
		}
		else
		{
			if(Mesh_Filter.sharedMesh == null || Mesh_Filter.sharedMesh.vertexCount == 0)
			{
				// No mesh has been created for this animation yet
				SetText(m_text);
			}
			
			m_running = true;
			m_paused = false;
		}
	}
	
	IEnumerator PlayAnimationAfterDelay(float delay)
	{
		if(Mesh_Filter.mesh == null || Mesh_Filter.mesh.vertexCount == 0)
		{
			// No mesh has been created for this animation yet
			SetText(m_text);
		}
		
		yield return new WaitForSeconds(delay);
		
		m_running = true;
		m_paused = false;
	}
	
	void Update()
	{
		if(m_running && !m_paused)
		{
			UpdateAnimation(m_time_type == AnimationTime.GAME_TIME ? Time.deltaTime : Time.realtimeSinceStartup - m_last_time);
			
			if(m_time_type == AnimationTime.REAL_TIME)
			{
				m_last_time = Time.realtimeSinceStartup;
			}
		}
	}
	
	// Reset animation to starting state
	public void ResetAnimation()
	{
		m_running = false;
		m_paused = false;
		SetAnimationState(0, 0);
	}
	
	void GetCharacterInfo(char m_character, out CustomCharacterInfo char_info)
	{
		char_info = null;
		
#if UNITY_4_0
		if(m_font != null)
		{
			CharacterInfo font_char_info = new CharacterInfo();
			m_font.GetCharacterInfo(m_character, out font_char_info);
			
			char_info = new CustomCharacterInfo();
			char_info.flipped = font_char_info.flipped;
			char_info.uv = font_char_info.uv;
			char_info.vert = font_char_info.vert;
			char_info.width = font_char_info.width;
			
			// Scale char_info values
			char_info.vert.x /= FontScale;
			char_info.vert.y /= FontScale;
			char_info.vert.width /= FontScale;
			char_info.vert.height /= FontScale;
			char_info.width /= FontScale;
		}
#endif		
		
		if(m_font_data_file != null && char_info == null)
		{
			char_info = new CustomCharacterInfo();
			
			if(m_custom_font_data == null || !m_font_data_file.name.Equals(m_current_font_data_file_name))
			{
				// Setup m_custom_font_data for the custom font.
				if(m_font_data_file.text.Substring(0,5).Equals("<?xml"))
				{
					// Text file is in xml format
					
					m_current_font_data_file_name = m_font_data_file.name;
					m_custom_font_data = new CustomFontCharacterData();
					
					XmlTextReader reader = new XmlTextReader(new StringReader(m_font_data_file.text));
					
					int texture_width = 0;
					int texture_height = 0;
					int uv_x, uv_y;
					float width, height, xoffset, yoffset, xadvance;
					CustomCharacterInfo character_info;
					
					while(reader.Read())
					{
						if(reader.IsStartElement())
						{
							if(reader.Name.Equals("common"))
							{
								texture_width = int.Parse(reader.GetAttribute("scaleW"));
								texture_height = int.Parse(reader.GetAttribute("scaleH"));
							}
							else if(reader.Name.Equals("char"))
							{
								uv_x = int.Parse(reader.GetAttribute("x"));
								uv_y = int.Parse(reader.GetAttribute("y"));
								width = float.Parse(reader.GetAttribute("width"));
								height = float.Parse(reader.GetAttribute("height"));
								xoffset = float.Parse(reader.GetAttribute("xoffset"));
								yoffset = float.Parse(reader.GetAttribute("yoffset"));
								xadvance = float.Parse(reader.GetAttribute("xadvance"));
								
								character_info = new CustomCharacterInfo();
								character_info.flipped = false;
								character_info.uv = new Rect((float) uv_x / (float) texture_width, 1 - ((float)uv_y / (float)texture_height) - (float)height/(float)texture_height, (float)width/(float)texture_width, (float)height/(float)texture_height);
								character_info.vert = new Rect(xoffset,-yoffset,width, -height);
								character_info.width = xadvance;
								
								m_custom_font_data.m_character_infos.Add( int.Parse(reader.GetAttribute("id")), character_info);
							}
						}
					}
				}
				else if(m_font_data_file.text.Substring(0,4).Equals("info"))
				{
					// Plain txt format
					m_current_font_data_file_name = m_font_data_file.name;
					m_custom_font_data = new CustomFontCharacterData();
					
					int texture_width = 0;
					int texture_height = 0;
					int uv_x, uv_y;
					float width, height, xoffset, yoffset, xadvance;
					CustomCharacterInfo character_info;
					string[] data_fields;
					
					string[] text_lines = m_font_data_file.text.Split(new char[]{'\n'});
					
					foreach(string font_data in text_lines)
					{
						if(font_data.Length >= 5 && font_data.Substring(0,5).Equals("char "))
						{
							// character data line
							data_fields = ParseFieldData(font_data, new string[]{"id=", "x=", "y=", "width=", "height=", "xoffset=", "yoffset=", "xadvance="});
							uv_x = int.Parse(data_fields[1]);
							uv_y = int.Parse(data_fields[2]);
							width = float.Parse(data_fields[3]);
							height = float.Parse(data_fields[4]);
							xoffset = float.Parse(data_fields[5]);
							yoffset = float.Parse(data_fields[6]);
							xadvance = float.Parse(data_fields[7]);
							
							character_info = new CustomCharacterInfo();
							character_info.flipped = false;
							character_info.uv = new Rect((float) uv_x / (float) texture_width, 1 - ((float)uv_y / (float)texture_height) - (float)height/(float)texture_height, (float)width/(float)texture_width, (float)height/(float)texture_height);
							character_info.vert = new Rect(xoffset,-yoffset +1,width, -height);
							character_info.width = xadvance;
							
							m_custom_font_data.m_character_infos.Add( int.Parse(data_fields[0]), character_info);
						}
						else if(font_data.Length >= 6 && font_data.Substring(0,6).Equals("common"))
						{
							data_fields = ParseFieldData(font_data, new string[]{"scaleW=", "scaleH="});
							texture_width = int.Parse(data_fields[0]);
							texture_height = int.Parse(data_fields[1]);
						}
					}
				}
				
			}
			
			if(m_custom_font_data.m_character_infos.ContainsKey((int)m_character))
			{
				char_info = ((CustomCharacterInfo) m_custom_font_data.m_character_infos[(int)m_character]).ScaleClone(FontScale);
			}
			
		}
		else if(char_info == null)
		{
			char_info = new CustomCharacterInfo();
		}
		
		
		
		
//		Debug.LogError("Character : '" + m_character);
//		Debug.LogError(char_info.ToString());
	}
	
	string[] ParseFieldData(string data_string, string[] fields)
	{
		string[] data_values = new string[fields.Length];
		int count = 0, data_start_idx, data_end_idx;
		
		foreach(string field_name in fields)
		{
			data_start_idx = data_string.IndexOf(field_name) + field_name.Length;
			data_end_idx = data_string.IndexOf(" ", data_start_idx);
			
			data_values[count] = data_string.Substring(data_start_idx, data_end_idx - data_start_idx);
			
			count++;
		}
		
		return data_values;
	}
	
	public void SetText(string new_text)
	{
		if(m_running)
		{
			Debug.LogWarning("Can't change text while animation is running");
			return;
		}
		
		m_text = new_text;
		
		if(m_renderer == null)
		{
			m_renderer = this.GetComponent<Renderer>();
		}
		
		bool setup_correctly = false;
		
		// Automatically assign the font material to the renderer if its not already set
		if((m_renderer.sharedMaterial == null || m_renderer.sharedMaterial != m_font_material) && m_font_material != null)
		{
			m_renderer.sharedMaterial = m_font_material;
		}
#if UNITY_4_0		
		else if(m_font != null)
		{
			if(m_renderer.sharedMaterial == null || m_renderer.sharedMaterial != m_font_material)
			{
				m_font_material = m_font.material;
				m_renderer.sharedMaterial = m_font_material;
			}
			
			if(m_renderer.sharedMaterial != null)
			{
				setup_correctly = true;
			}
		}
		
#endif
		
		if(!setup_correctly && (m_renderer.sharedMaterial == null || m_font_data_file == null))
		{
			// Incorrectly setup font information
			Debug.LogWarning("SetText() : Incomplete font setup information. Check that you've assigned your font files in the inspector.");
			return;
		}
		
		
		// preserve existing letter instances if possible
		LetterSetup[] prev_letters = m_letters;
		float x_offset = 0;
		float y_offset = 0;
		int num_visible_chars = m_text.Replace(" ", "").Length;
		int text_length = 0;
		
		m_letters= new LetterSetup[num_visible_chars];
		text_length = m_text.Length;
	
		CustomCharacterInfo[] char_infos = new CustomCharacterInfo[m_text.Length];
		CustomCharacterInfo char_info;
		TextSizeData text_data = new TextSizeData();
		
		for(int letter_idx=0; letter_idx < text_length; letter_idx++)
		{
			GetCharacterInfo(m_text[letter_idx], out char_info);
			
			if(char_info != null)
			{
				// handle vertical text white space
				if(m_text[letter_idx].Equals(' ') && m_display_axis == TextDisplayAxis.VERTICAL)
				{
					char_info.vert.height = -char_info.width;
				}
				
				if(m_display_axis == TextDisplayAxis.HORIZONTAL)
				{
					if(letter_idx == 0 || char_info.vert.y > text_data.y_max)
					{
						text_data.y_max = char_info.vert.y;
					}
					if(letter_idx == 0 || char_info.vert.y + char_info.vert.height < text_data.y_min)
					{
						text_data.y_min = char_info.vert.y + char_info.vert.height;
					}
					
					// increment the text width by the letter progress width, and then full mesh width for last letter.
					text_data.text_width += letter_idx == text_length - 1 ? char_info.vert.width + char_info.vert.x : char_info.width;
				}
				else
				{
					if(letter_idx == 0 || char_info.vert.x + char_info.vert.width > text_data.x_max)
					{
						text_data.x_max = char_info.vert.x + char_info.vert.width;
					}
					if(letter_idx == 0 || char_info.vert.x < text_data.x_min)
					{
						text_data.x_min = char_info.vert.x;
					}
					
					text_data.text_height += char_info.vert.height;
				}
			}
			
			char_infos[letter_idx] = char_info;
		}
		
		if(m_display_axis == TextDisplayAxis.HORIZONTAL)
		{
			text_data.text_height = Mathf.Abs( text_data.y_max - text_data.y_min );
		}
		else
		{
			text_data.text_height *= -1;
			text_data.text_width = Mathf.Abs( text_data.x_max - text_data.x_min );
		}
		
		int letter_count = 0;
		for(int letter_idx = 0; letter_idx < char_infos.Length; letter_idx++)
		{
			char_info = char_infos[letter_idx];
			
//			Debug.Log("letter : " + m_text[letter_idx] + " , " + char_info.vert.x + "," + char_info.vert.y + " : " + char_info.vert.width + "," + char_info.vert.height);

			if(char_info != null)
			{
				// handle white space
				if(m_text[letter_idx].Equals(' '))
				{
					x_offset += char_info.width;
					y_offset += -char_info.width;
				}
				else
				{
					if(letter_count < prev_letters.Length
						&& prev_letters[letter_count].m_character.Equals(m_text[letter_idx].ToString())
						&& prev_letters[letter_count].m_letter_idx == letter_idx
						&& prev_letters[letter_count].m_mesh != null)
					{
						// Use same LetterSetup from before
						m_letters[letter_count] = prev_letters[letter_count];
						
						// Remove instance from previous letters list
						prev_letters[letter_count] = null;
						
						// position the letter offset again, incase it has changed from previous letters changing.
						Vector3 base_offset = m_display_axis == TextDisplayAxis.HORIZONTAL ? new Vector3(x_offset, 0, 0) : new Vector3(0, y_offset, 0);
						m_letters[letter_count].m_base_offset = base_offset;
						m_letters[letter_count].SetupLetterMesh(char_info);
						m_letters[letter_count].SetupBaseOffsets(m_text_anchor, m_display_axis, text_data);
					}
					else
					{
						Mesh mesh = new Mesh();
						
						// Setup Mesh UV co-ords and triangles (and fill in placeholder vertices)
						Rect uv_data = char_info.uv;
						mesh.vertices = new Vector3[]{Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero};
						mesh.uv = new Vector2[]{ new Vector2(uv_data.x + uv_data.width, uv_data.y + uv_data.height), new Vector2(uv_data.x, uv_data.y + uv_data.height), new Vector2(uv_data.x, uv_data.y), new Vector2(uv_data.x + uv_data.width, uv_data.y)};
						mesh.triangles = new int[]{2,1,0, 3,2,0};
						
						LetterAnimation letter_animation = null;
						if(letter_count < prev_letters.Length)
						{	
							letter_animation = prev_letters[letter_count].m_letter_animation;
						}
						else if(letter_count > 0 && letter_count-1 < prev_letters.Length && prev_letters[letter_count-1] != null)
						{
							// use same animation as previous letter
							letter_animation = prev_letters[letter_count - 1].m_letter_animation.Clone(letter_count, text_length);
						}
						
						Vector3 base_offset = m_display_axis == TextDisplayAxis.HORIZONTAL ? new Vector3(x_offset, 0, 0) : new Vector3(0, y_offset, 0);
						m_letters[letter_count] = new LetterSetup("" + m_text[letter_idx], letter_idx, text_length, mesh, base_offset, char_info, letter_animation, m_text_anchor, m_display_axis, text_data);
					}
					
					letter_count ++;
					
					x_offset += char_info.width + (m_px_offset.x / FontScale);
					y_offset += char_info.vert.height + (-m_px_offset.y / FontScale);
				}
			}
		}
		
		foreach(LetterSetup old_letter in prev_letters)
		{
			if(old_letter != null)
			{
				// Letter wasn't used in new text setup; delete it's mesh instance.
				if(Application.isPlaying)
				{
					Destroy(old_letter.m_mesh);
				}
				else
				{
					DestroyImmediate(old_letter.m_mesh);
				}
			}
		}
		
		SetAnimationState(0, 0);
	}
	
	public bool UpdateAnimation(float delta_time)
	{
		m_animation_timer += delta_time;
		
		if(UpdateMesh())
		{
			m_running = false;
		}
		
		return m_running;
	}
	
	public void SetAnimationState(int action_idx, float action_progress)
	{
		// Copy latest master animation state to all letters
		m_animation_timer = 0;
		m_lowest_action_progress = -1;
		int count = 0;
		int num_letters = m_letters.Length;
		foreach(LetterSetup letter in m_letters)
		{
			letter.m_waiting_to_sync = false;
			letter.m_letter_animation = m_master_animation.Clone(count, num_letters);
			letter.Reset(num_letters == 1 ? 0 : (float)count / ((float)num_letters - 1f));
			count ++;
		}
		
		CombineInstance[] combine = new CombineInstance[m_letters.Length];
		count = 0;
		
		foreach(LetterSetup letter in m_letters)
		{
			letter.SetMeshState(action_idx, action_progress);
			
			combine[count].mesh = letter.m_mesh;
			
			count ++;
		}
		
		if(Mesh_Filter.sharedMesh == null)
		{
			Mesh_Filter.sharedMesh = new Mesh();
		}
		Mesh_Filter.sharedMesh.CombineMeshes(combine, true, false);
	}
	
	bool UpdateMesh()
	{
		bool all_letter_anims_finished = true;
		CombineInstance[] combine = new CombineInstance[m_letters.Length];
		int count = 0;
		
		int lowest_action_progress = -1;
		foreach(LetterSetup letter in m_letters)
		{
			if(lowest_action_progress == -1 || letter.m_letter_animation.ActionProgress < lowest_action_progress)
			{
				lowest_action_progress = letter.m_letter_animation.ActionProgress;
			}
			
			if(letter.AnimateMesh(m_animation_timer, m_text_anchor, m_lowest_action_progress))
			{
				// Letter animation is still active, so not all letters are finished
				all_letter_anims_finished = false;
			}
			
			combine[count].mesh = letter.m_mesh;
			
			count ++;
		}
		
		if(lowest_action_progress > m_lowest_action_progress)
		{
			m_lowest_action_progress = lowest_action_progress;
		}
		
		Mesh_Filter.sharedMesh.CombineMeshes(combine, true, false);
		
		return all_letter_anims_finished;
	}
	
	void OnDestroy()
	{
		// Destroy all letter mesh instances
		foreach(LetterSetup letter in m_letters)
		{
			Destroy(letter.m_mesh);
		}
		
		// Destroy shared mesh instance.
		Destroy(Mesh_Filter.sharedMesh);
    }
	
	// Lerp function that handles progress value going over 1
	public static Vector3 Vector3Lerp(Vector3 from_vec, Vector3 to_vec, float progress)
	{
		if(progress <= 1)
		{
			return Vector3.Lerp(from_vec, to_vec, progress);
		}
		else
		{
			return from_vec + Vector3.Scale((to_vec - from_vec), Vector3.one * progress);
		}
	}
	
	
	public static float GetEaseProgress(EasingEquation ease_type, float linear_progress)
	{
		switch(ease_type)
		{
		case EasingEquation.Linear:
			return linear_progress;
		case EasingEquation.BackEaseIn:
			return EffectManager.BackEaseIn(linear_progress, 0, 1, 1);
			
		case EasingEquation.BackEaseInOut:
			return EffectManager.BackEaseInOut(linear_progress, 0, 1, 1);
		case EasingEquation.BackEaseOut:
			return EffectManager.BackEaseOut(linear_progress, 0, 1, 1);
		case EasingEquation.BackEaseOutIn:
			return EffectManager.BackEaseOutIn(linear_progress, 0, 1, 1);
		case EasingEquation.BounceEaseIn:
			return EffectManager.BounceEaseIn(linear_progress, 0, 1, 1);
		case EasingEquation.BounceEaseInOut:
			return EffectManager.BounceEaseInOut(linear_progress, 0, 1, 1);
		case EasingEquation.BounceEaseOut:
			return EffectManager.BounceEaseOut(linear_progress, 0, 1, 1);
		case EasingEquation.BounceEaseOutIn:
			return EffectManager.BounceEaseOutIn(linear_progress, 0, 1, 1);
		case EasingEquation.CircEaseIn:
			return EffectManager.CircEaseIn(linear_progress, 0, 1, 1);
		case EasingEquation.CircEaseInOut:
			return EffectManager.CircEaseInOut(linear_progress, 0, 1, 1);
		case EasingEquation.CircEaseOut:
			return EffectManager.CircEaseOut(linear_progress, 0, 1, 1);
		case EasingEquation.CircEaseOutIn:
			return EffectManager.CircEaseOutIn(linear_progress, 0, 1, 1);
		case EasingEquation.CubicEaseIn:
			return EffectManager.CubicEaseIn(linear_progress, 0, 1, 1);
		case EasingEquation.CubicEaseInOut:
			return EffectManager.CubicEaseInOut(linear_progress, 0, 1, 1);
		case EasingEquation.CubicEaseOut:
			return EffectManager.CubicEaseOut(linear_progress, 0, 1, 1);
		case EasingEquation.CubicEaseOutIn:
			return EffectManager.CubicEaseOutIn(linear_progress, 0, 1, 1);
		case EasingEquation.ElasticEaseIn:
			return EffectManager.ElasticEaseIn(linear_progress, 0, 1, 1);
			
		case EasingEquation.ElasticEaseInOut:
			return EffectManager.ElasticEaseInOut(linear_progress, 0, 1, 1);
		case EasingEquation.ElasticEaseOut:
			return EffectManager.ElasticEaseOut(linear_progress, 0, 1, 1);
		case EasingEquation.ElasticEaseOutIn:
			return EffectManager.ElasticEaseOutIn(linear_progress, 0, 1, 1);
		case EasingEquation.ExpoEaseIn:
			return EffectManager.ExpoEaseIn(linear_progress, 0, 1, 1);
		case EasingEquation.ExpoEaseInOut:
			return EffectManager.ExpoEaseInOut(linear_progress, 0, 1, 1);
		case EasingEquation.ExpoEaseOut:
			return EffectManager.ExpoEaseOut(linear_progress, 0, 1, 1);
		case EasingEquation.ExpoEaseOutIn:
			return EffectManager.ExpoEaseOutIn(linear_progress, 0, 1, 1);
		case EasingEquation.QuadEaseIn:
			return EffectManager.QuadEaseIn(linear_progress, 0, 1, 1);
		case EasingEquation.QuadEaseInOut:
			return EffectManager.QuadEaseInOut(linear_progress, 0, 1, 1);
		case EasingEquation.QuadEaseOut:
			return EffectManager.QuadEaseOut(linear_progress, 0, 1, 1);
		case EasingEquation.QuadEaseOutIn:
			return EffectManager.QuadEaseOutIn(linear_progress, 0, 1, 1);
		case EasingEquation.QuartEaseIn:
			return EffectManager.QuartEaseIn(linear_progress, 0, 1, 1);
		case EasingEquation.QuartEaseInOut:
			return EffectManager.QuartEaseInOut(linear_progress, 0, 1, 1);
		case EasingEquation.QuartEaseOut:
			return EffectManager.QuartEaseOut(linear_progress, 0, 1, 1);
		case EasingEquation.QuartEaseOutIn:
			return EffectManager.QuartEaseOutIn(linear_progress, 0, 1, 1);
		case EasingEquation.QuintEaseIn:
			return EffectManager.QuintEaseIn(linear_progress, 0, 1, 1);
		case EasingEquation.QuintEaseInOut:
			return EffectManager.QuintEaseInOut(linear_progress, 0, 1, 1);
		case EasingEquation.QuintEaseOut:
			return EffectManager.QuintEaseOut(linear_progress, 0, 1, 1);
		case EasingEquation.QuintEaseOutIn:
			return EffectManager.QuintEaseOutIn(linear_progress, 0, 1, 1);
			
		case EasingEquation.SineEaseIn:
			return EffectManager.SineEaseIn(linear_progress, 0, 1, 1);
		case EasingEquation.SineEaseInOut:
			return EffectManager.SineEaseInOut(linear_progress, 0, 1, 1);
		case EasingEquation.SineEaseOut:
			return EffectManager.SineEaseOut(linear_progress, 0, 1, 1);
		case EasingEquation.SineEaseOutIn:
			return EffectManager.SineEaseOutIn(linear_progress, 0, 1, 1);
			
		default :
			return linear_progress;
		}
	}
	
	public static EasingEquation GetEaseTypeOpposite(EasingEquation ease_type)
	{
		switch(ease_type)
		{
		case EasingEquation.Linear:
			return EasingEquation.Linear;
		case EasingEquation.BackEaseIn:
			return EasingEquation.BackEaseOut;
		case EasingEquation.BackEaseInOut:
			return EasingEquation.BackEaseOutIn;
		case EasingEquation.BackEaseOut:
			return EasingEquation.BackEaseIn;
		case EasingEquation.BackEaseOutIn:
			return EasingEquation.BackEaseInOut;
		case EasingEquation.BounceEaseIn:
			return EasingEquation.BounceEaseOut;
			
			
		case EasingEquation.BounceEaseInOut:
			return EasingEquation.BounceEaseOutIn;
		case EasingEquation.BounceEaseOut:
			return EasingEquation.BounceEaseIn;
		case EasingEquation.BounceEaseOutIn:
			return EasingEquation.BounceEaseInOut;
		case EasingEquation.CircEaseIn:
			return EasingEquation.CircEaseOut;
			
			
		case EasingEquation.CircEaseInOut:
			return EasingEquation.CircEaseOutIn;
		case EasingEquation.CircEaseOut:
			return EasingEquation.CircEaseIn;
			
			
		case EasingEquation.CircEaseOutIn:
			return EasingEquation.CircEaseInOut;
		case EasingEquation.CubicEaseIn:
			return EasingEquation.CubicEaseOut;
		case EasingEquation.CubicEaseInOut:
			return EasingEquation.CubicEaseOutIn;
		case EasingEquation.CubicEaseOut:
			return EasingEquation.CubicEaseIn;
		case EasingEquation.CubicEaseOutIn:
			return EasingEquation.CubicEaseInOut;
		case EasingEquation.ElasticEaseIn:
			return EasingEquation.ElasticEaseOut;
			
		case EasingEquation.ElasticEaseInOut:
			return EasingEquation.ElasticEaseOutIn;
		case EasingEquation.ElasticEaseOut:
			return EasingEquation.ElasticEaseIn;
		case EasingEquation.ElasticEaseOutIn:
			return EasingEquation.ElasticEaseInOut;
		case EasingEquation.ExpoEaseIn:
			return EasingEquation.ExpoEaseOut;
		case EasingEquation.ExpoEaseInOut:
			return EasingEquation.ExpoEaseOutIn;
		case EasingEquation.ExpoEaseOut:
			return EasingEquation.ExpoEaseIn;
		case EasingEquation.ExpoEaseOutIn:
			return EasingEquation.ExpoEaseInOut;
		case EasingEquation.QuadEaseIn:
			return EasingEquation.QuadEaseOut;
			
			
		case EasingEquation.QuadEaseInOut:
			return EasingEquation.QuadEaseOutIn;
		case EasingEquation.QuadEaseOut:
			return EasingEquation.QuadEaseIn;
		case EasingEquation.QuadEaseOutIn:
			return EasingEquation.QuadEaseInOut;
		case EasingEquation.QuartEaseIn:
			return EasingEquation.QuartEaseOut;
		case EasingEquation.QuartEaseInOut:
			return EasingEquation.QuartEaseOutIn;
		case EasingEquation.QuartEaseOut:
			return EasingEquation.QuartEaseIn;
		case EasingEquation.QuartEaseOutIn:
			return EasingEquation.QuartEaseInOut;
		case EasingEquation.QuintEaseIn:
			return EasingEquation.QuintEaseOut;
		case EasingEquation.QuintEaseInOut:
			return EasingEquation.QuintEaseOutIn;
		case EasingEquation.QuintEaseOut:
			return EasingEquation.QuintEaseIn;
		case EasingEquation.QuintEaseOutIn:
			return EasingEquation.QuintEaseInOut;
			
		case EasingEquation.SineEaseIn:
			return EasingEquation.SineEaseOut;
		case EasingEquation.SineEaseInOut:
			return EasingEquation.SineEaseOutIn;
		case EasingEquation.SineEaseOut:
			return EasingEquation.SineEaseIn;
		case EasingEquation.SineEaseOutIn:
			return EasingEquation.SineEaseInOut;
			
		default :
			return EasingEquation.Linear;
		}
	}
	
	
	/* EASING FUNCTIONS */
	
	#region Linear

    /// <summary>
    /// Easing equation function for a simple linear tweening, with no easing.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float Linear( float t, float b, float c, float d )
    {
        return c * t / d + b;
    }

    #endregion

    #region Expo

    /// <summary>
    /// Easing equation function for an exponential (2^t) easing out: 
    /// decelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float ExpoEaseOut( float t, float b, float c, float d )
    {
        return ( t == d ) ? b + c : c * ( -Mathf.Pow( 2, -10 * t / d ) + 1 ) + b;
    }

    /// <summary>
    /// Easing equation function for an exponential (2^t) easing in: 
    /// accelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float ExpoEaseIn( float t, float b, float c, float d )
    {
        return ( t == 0 ) ? b : c * Mathf.Pow( 2, 10 * ( t / d - 1 ) ) + b;
    }

    /// <summary>
    /// Easing equation function for an exponential (2^t) easing in/out: 
    /// acceleration until halfway, then deceleration.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float ExpoEaseInOut( float t, float b, float c, float d )
    {
        if ( t == 0 )
            return b;

        if ( t == d )
            return b + c;

        if ( ( t /= d / 2 ) < 1 )
            return c / 2 * Mathf.Pow( 2, 10 * ( t - 1 ) ) + b;

        return c / 2 * ( -Mathf.Pow( 2, -10 * --t ) + 2 ) + b;
    }

    /// <summary>
    /// Easing equation function for an exponential (2^t) easing out/in: 
    /// deceleration until halfway, then acceleration.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float ExpoEaseOutIn( float t, float b, float c, float d )
    {
        if ( t < d / 2 )
            return ExpoEaseOut( t * 2, b, c / 2, d );

        return ExpoEaseIn( ( t * 2 ) - d, b + c / 2, c / 2, d );
    }

    #endregion

    #region Circular

    /// <summary>
    /// Easing equation function for a circular (sqrt(1-t^2)) easing out: 
    /// decelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float CircEaseOut( float t, float b, float c, float d )
    {
        return c * Mathf.Sqrt( 1 - ( t = t / d - 1 ) * t ) + b;
    }

    /// <summary>
    /// Easing equation function for a circular (sqrt(1-t^2)) easing in: 
    /// accelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float CircEaseIn( float t, float b, float c, float d )
    {
        return -c * ( Mathf.Sqrt( 1 - ( t /= d ) * t ) - 1 ) + b;
    }

    /// <summary>
    /// Easing equation function for a circular (sqrt(1-t^2)) easing in/out: 
    /// acceleration until halfway, then deceleration.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float CircEaseInOut( float t, float b, float c, float d )
    {
        if ( ( t /= d / 2 ) < 1 )
            return -c / 2 * ( Mathf.Sqrt( 1 - t * t ) - 1 ) + b;

        return c / 2 * ( Mathf.Sqrt( 1 - ( t -= 2 ) * t ) + 1 ) + b;
    }

    /// <summary>
    /// Easing equation function for a circular (sqrt(1-t^2)) easing in/out: 
    /// acceleration until halfway, then deceleration.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float CircEaseOutIn( float t, float b, float c, float d )
    {
        if ( t < d / 2 )
            return CircEaseOut( t * 2, b, c / 2, d );

        return CircEaseIn( ( t * 2 ) - d, b + c / 2, c / 2, d );
    }

    #endregion

    #region Quad

    /// <summary>
    /// Easing equation function for a quadratic (t^2) easing out: 
    /// decelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float QuadEaseOut( float t, float b, float c, float d )
    {
        return -c * ( t /= d ) * ( t - 2 ) + b;
    }

    /// <summary>
    /// Easing equation function for a quadratic (t^2) easing in: 
    /// accelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float QuadEaseIn( float t, float b, float c, float d )
    {
        return c * ( t /= d ) * t + b;
    }

    /// <summary>
    /// Easing equation function for a quadratic (t^2) easing in/out: 
    /// acceleration until halfway, then deceleration.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float QuadEaseInOut( float t, float b, float c, float d )
    {
        if ( ( t /= d / 2 ) < 1 )
            return c / 2 * t * t + b;

        return -c / 2 * ( ( --t ) * ( t - 2 ) - 1 ) + b;
    }

    /// <summary>
    /// Easing equation function for a quadratic (t^2) easing out/in: 
    /// deceleration until halfway, then acceleration.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float QuadEaseOutIn( float t, float b, float c, float d )
    {
        if ( t < d / 2 )
            return QuadEaseOut( t * 2, b, c / 2, d );

        return QuadEaseIn( ( t * 2 ) - d, b + c / 2, c / 2, d );
    }

    #endregion

    #region Sine

    /// <summary>
    /// Easing equation function for a sinusoidal (sin(t)) easing out: 
    /// decelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float SineEaseOut( float t, float b, float c, float d )
    {
        return c * Mathf.Sin( t / d * ( Mathf.PI / 2 ) ) + b;
    }

    /// <summary>
    /// Easing equation function for a sinusoidal (sin(t)) easing in: 
    /// accelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float SineEaseIn( float t, float b, float c, float d )
    {
        return -c * Mathf.Cos( t / d * ( Mathf.PI / 2 ) ) + c + b;
    }

    /// <summary>
    /// Easing equation function for a sinusoidal (sin(t)) easing in/out: 
    /// acceleration until halfway, then deceleration.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float SineEaseInOut( float t, float b, float c, float d )
    {
        if ( ( t /= d / 2 ) < 1 )
            return c / 2 * ( Mathf.Sin( Mathf.PI * t / 2 ) ) + b;

        return -c / 2 * ( Mathf.Cos( Mathf.PI * --t / 2 ) - 2 ) + b;
    }

    /// <summary>
    /// Easing equation function for a sinusoidal (sin(t)) easing in/out: 
    /// deceleration until halfway, then acceleration.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float SineEaseOutIn( float t, float b, float c, float d )
    {
        if ( t < d / 2 )
            return SineEaseOut( t * 2, b, c / 2, d );

        return SineEaseIn( ( t * 2 ) - d, b + c / 2, c / 2, d );
    }

    #endregion

    #region Cubic

    /// <summary>
    /// Easing equation function for a cubic (t^3) easing out: 
    /// decelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float CubicEaseOut( float t, float b, float c, float d )
    {
        return c * ( ( t = t / d - 1 ) * t * t + 1 ) + b;
    }

    /// <summary>
    /// Easing equation function for a cubic (t^3) easing in: 
    /// accelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float CubicEaseIn( float t, float b, float c, float d )
    {
        return c * ( t /= d ) * t * t + b;
    }

    /// <summary>
    /// Easing equation function for a cubic (t^3) easing in/out: 
    /// acceleration until halfway, then deceleration.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float CubicEaseInOut( float t, float b, float c, float d )
    {
        if ( ( t /= d / 2 ) < 1 )
            return c / 2 * t * t * t + b;

        return c / 2 * ( ( t -= 2 ) * t * t + 2 ) + b;
    }

    /// <summary>
    /// Easing equation function for a cubic (t^3) easing out/in: 
    /// deceleration until halfway, then acceleration.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float CubicEaseOutIn( float t, float b, float c, float d )
    {
        if ( t < d / 2 )
            return CubicEaseOut( t * 2, b, c / 2, d );

        return CubicEaseIn( ( t * 2 ) - d, b + c / 2, c / 2, d );
    }

    #endregion

    #region Quartic

    /// <summary>
    /// Easing equation function for a quartic (t^4) easing out: 
    /// decelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float QuartEaseOut( float t, float b, float c, float d )
    {
        return -c * ( ( t = t / d - 1 ) * t * t * t - 1 ) + b;
    }

    /// <summary>
    /// Easing equation function for a quartic (t^4) easing in: 
    /// accelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float QuartEaseIn( float t, float b, float c, float d )
    {
        return c * ( t /= d ) * t * t * t + b;
    }

    /// <summary>
    /// Easing equation function for a quartic (t^4) easing in/out: 
    /// acceleration until halfway, then deceleration.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float QuartEaseInOut( float t, float b, float c, float d )
    {
        if ( ( t /= d / 2 ) < 1 )
            return c / 2 * t * t * t * t + b;

        return -c / 2 * ( ( t -= 2 ) * t * t * t - 2 ) + b;
    }

    /// <summary>
    /// Easing equation function for a quartic (t^4) easing out/in: 
    /// deceleration until halfway, then acceleration.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float QuartEaseOutIn( float t, float b, float c, float d )
    {
        if ( t < d / 2 )
            return QuartEaseOut( t * 2, b, c / 2, d );

        return QuartEaseIn( ( t * 2 ) - d, b + c / 2, c / 2, d );
    }

    #endregion

    #region Quintic

    /// <summary>
    /// Easing equation function for a quintic (t^5) easing out: 
    /// decelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float QuintEaseOut( float t, float b, float c, float d )
    {
        return c * ( ( t = t / d - 1 ) * t * t * t * t + 1 ) + b;
    }

    /// <summary>
    /// Easing equation function for a quintic (t^5) easing in: 
    /// accelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float QuintEaseIn( float t, float b, float c, float d )
    {
        return c * ( t /= d ) * t * t * t * t + b;
    }

    /// <summary>
    /// Easing equation function for a quintic (t^5) easing in/out: 
    /// acceleration until halfway, then deceleration.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float QuintEaseInOut( float t, float b, float c, float d )
    {
        if ( ( t /= d / 2 ) < 1 )
            return c / 2 * t * t * t * t * t + b;
        return c / 2 * ( ( t -= 2 ) * t * t * t * t + 2 ) + b;
    }

    /// <summary>
    /// Easing equation function for a quintic (t^5) easing in/out: 
    /// acceleration until halfway, then deceleration.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float QuintEaseOutIn( float t, float b, float c, float d )
    {
        if ( t < d / 2 )
            return QuintEaseOut( t * 2, b, c / 2, d );
        return QuintEaseIn( ( t * 2 ) - d, b + c / 2, c / 2, d );
    }

    #endregion

    #region Elastic

    /// <summary>
    /// Easing equation function for an elastic (exponentially decaying sine wave) easing out: 
    /// decelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float ElasticEaseOut( float t, float b, float c, float d )
    {
        if ( ( t /= d ) == 1 )
            return b + c;

        float p = d * 0.3f;
        float s = p / 4;

        return ( c * Mathf.Pow( 2, -10 * t ) * Mathf.Sin( ( t * d - s ) * ( 2 * Mathf.PI ) / p ) + c + b );
    }

    /// <summary>
    /// Easing equation function for an elastic (exponentially decaying sine wave) easing in: 
    /// accelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float ElasticEaseIn( float t, float b, float c, float d )
    {
        if ( ( t /= d ) == 1 )
            return b + c;

        float p = d * 0.3f;
        float s = p / 4;

        return -( c * Mathf.Pow( 2, 10 * ( t -= 1 ) ) * Mathf.Sin( ( t * d - s ) * ( 2 * Mathf.PI ) / p ) ) + b;
    }

    /// <summary>
    /// Easing equation function for an elastic (exponentially decaying sine wave) easing in/out: 
    /// acceleration until halfway, then deceleration.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float ElasticEaseInOut( float t, float b, float c, float d )
    {
        if ( ( t /= d / 2f ) == 2 )
            return b + c;

        float p = d * ( 0.3f * 1.5f );
        float s = p / 4;

        if ( t < 1 )
            return -0.5f * ( c * Mathf.Pow( 2, 10 * ( t -= 1 ) ) * Mathf.Sin( ( t * d - s ) * ( 2 * Mathf.PI ) / p ) ) + b;
        return c * Mathf.Pow( 2, -10 * ( t -= 1 ) ) * Mathf.Sin( ( t * d - s ) * ( 2 * Mathf.PI ) / p ) * 0.5f + c + b;
    }

    /// <summary>
    /// Easing equation function for an elastic (exponentially decaying sine wave) easing out/in: 
    /// deceleration until halfway, then acceleration.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float ElasticEaseOutIn( float t, float b, float c, float d )
    {
        if ( t < d / 2 )
            return ElasticEaseOut( t * 2, b, c / 2, d );
        return ElasticEaseIn( ( t * 2 ) - d, b + c / 2, c / 2, d );
    }

    #endregion

    #region Bounce

    /// <summary>
    /// Easing equation function for a bounce (exponentially decaying parabolic bounce) easing out: 
    /// decelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float BounceEaseOut( float t, float b, float c, float d )
    {
        if ( ( t /= d ) < ( 1 / 2.75f ) )
            return c * ( 7.5625f * t * t ) + b;
        else if ( t < ( 2 / 2.75f ) )
            return c * ( 7.5625f * ( t -= ( 1.5f / 2.75f ) ) * t + 0.75f ) + b;
        else if ( t < ( 2.5f / 2.75f ) )
            return c * ( 7.5625f * ( t -= ( 2.25f / 2.75f ) ) * t + 0.9375f ) + b;
        else
            return c * ( 7.5625f * ( t -= ( 2.625f / 2.75f ) ) * t + .984375f ) + b;
    }

    /// <summary>
    /// Easing equation function for a bounce (exponentially decaying parabolic bounce) easing in: 
    /// accelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float BounceEaseIn( float t, float b, float c, float d )
    {
        return c - BounceEaseOut( d - t, 0, c, d ) + b;
    }

    /// <summary>
    /// Easing equation function for a bounce (exponentially decaying parabolic bounce) easing in/out: 
    /// acceleration until halfway, then deceleration.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float BounceEaseInOut( float t, float b, float c, float d )
    {
        if ( t < d / 2 )
            return BounceEaseIn( t * 2, 0, c, d ) * 0.5f + b;
        else
            return BounceEaseOut( t * 2 - d, 0, c, d ) * 0.5f + c * 0.5f + b;
    }

    /// <summary>
    /// Easing equation function for a bounce (exponentially decaying parabolic bounce) easing out/in: 
    /// deceleration until halfway, then acceleration.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float BounceEaseOutIn( float t, float b, float c, float d )
    {
        if ( t < d / 2 )
            return BounceEaseOut( t * 2, b, c / 2, d );
        return BounceEaseIn( ( t * 2 ) - d, b + c / 2, c / 2, d );
    }

    #endregion

    #region Back

    /// <summary>
    /// Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing out: 
    /// decelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float BackEaseOut( float t, float b, float c, float d )
    {
        return c * ( ( t = t / d - 1 ) * t * ( ( 1.70158f + 1 ) * t + 1.70158f ) + 1 ) + b;
    }

    /// <summary>
    /// Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing in: 
    /// accelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float BackEaseIn( float t, float b, float c, float d )
    {
        return c * ( t /= d ) * t * ( ( 1.70158f + 1 ) * t - 1.70158f ) + b;
    }

    /// <summary>
    /// Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing in/out: 
    /// acceleration until halfway, then deceleration.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float BackEaseInOut( float t, float b, float c, float d )
    {
        float s = 1.70158f;
        if ( ( t /= d / 2 ) < 1 )
            return c / 2 * ( t * t * ( ( ( s *= ( 1.525f ) ) + 1 ) * t - s ) ) + b;
        return c / 2 * ( ( t -= 2 ) * t * ( ( ( s *= ( 1.525f ) ) + 1 ) * t + s ) + 2 ) + b;
    }

    /// <summary>
    /// Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing out/in: 
    /// deceleration until halfway, then acceleration.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float BackEaseOutIn( float t, float b, float c, float d )
    {
        if ( t < d / 2 )
            return BackEaseOut( t * 2, b, c / 2, d );
        return BackEaseIn( ( t * 2 ) - d, b + c / 2, c / 2, d );
    }

    #endregion
}
