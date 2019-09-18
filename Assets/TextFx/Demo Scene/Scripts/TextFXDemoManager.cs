#define DISPLAY_GUI_CONTROLS

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EffectManagerPool
{
	public List<EffectManager> m_effect_pool;
	
	EffectManager m_prefab_effect;
	Transform m_parent;
	
	public EffectManagerPool(Transform parent, EffectManager prefab_effect)
	{
		m_prefab_effect = prefab_effect;
		m_parent = parent;
		
		m_effect_pool = new List<EffectManager>();
	}
	
	public EffectManager GetNextAvailable()
	{
		foreach(EffectManager effect in m_effect_pool)
		{
			if(!effect.enabled)
			{
#if UNITY_4_0				
				effect.gameObject.SetActive(true);
#else
				effect.gameObject.active = true;
#endif
				effect.enabled = true;
				return effect;
			}
		}
		
		// no available inactive effects. Need to instantiate a new one
		EffectManager new_effect = ((EffectManager) GameObject.Instantiate(m_prefab_effect));
		
		new_effect.transform.parent = m_parent;
		m_effect_pool.Add(new_effect);
		
		return new_effect;
	}
}

[System.Serializable]
public class DemoEffectSetup
{
	public EffectManager m_textfx_animation;
	public string m_text = "";
	public bool m_allow_controls = true;
	public EffectManager[] m_touch_effects;
	public string[] m_touch_strings;
	[HideInInspector]
	public int m_string_pool_idx = 0;
	[HideInInspector]
	public EffectManagerPool[] m_touch_effect_pools;
}

public class TextFXDemoManager : MonoBehaviour
{
	static Vector3 ANIMATION_BASE_OFFSET = new Vector3(0,0,12);		// animation offset from demo manager position
	static Vector3 ANIMATION_SPACING = new Vector3(50,0,0);	// Positional offset between each example animation.
	static float TRANSITION_TIME = 0.5f;
	
	Transform m_transform;
	int m_current_animation_index = -1;
	Touch[] m_screen_touches;
	Vector3 m_cursor_pos = Vector3.zero;
	
	public DemoEffectSetup[] m_demo_effects;
	public int m_start_anim_idx = 0;
	public float m_scene_time_scale = 1;
	
	DemoEffectSetup CurrentDemo { get { return m_current_animation_index < m_demo_effects.Length ? m_demo_effects[m_current_animation_index] : null; } }
	EffectManager CurrentAnim { get { return m_current_animation_index < m_demo_effects.Length ? m_demo_effects[m_current_animation_index].m_textfx_animation : null; } }
	
	// Use this for initialization
	void Start ()
	{
		m_transform = this.transform;
		
		// Position demo manager for chosen start animation
		m_transform.position = -ANIMATION_SPACING * m_start_anim_idx;
		
		// Position and reset all animation effects
		int effect_idx = 0;
		foreach(DemoEffectSetup effect in m_demo_effects)
		{
			effect.m_textfx_animation.gameObject.transform.localPosition = ANIMATION_BASE_OFFSET + ANIMATION_SPACING * effect_idx;
			effect.m_textfx_animation.ResetAnimation();
			
			effect_idx++;
		}
		
		// offset to starting animation
		StartCoroutine( FocusOnAnimation(m_start_anim_idx, false, true));
		
		// Play starting animation after delay
		CurrentAnim.SetText(CurrentDemo.m_text);
		CurrentAnim.PlayAnimation(1);
	}
	
	void Update()
	{
		m_screen_touches = Input.touches;
		bool touch_detected = false;
		
		// Read touch screen tap input
		if(m_screen_touches.Length > 0)
		{
			if(m_screen_touches.Length == 1)
			{
				if(m_screen_touches[0].phase == TouchPhase.Began)
				{
					m_cursor_pos = Input.mousePosition;
					touch_detected = true;
				}
			}
		}
		
		// Read mouse click input
		if(Input.GetMouseButtonDown(0))
		{
			m_cursor_pos = Input.mousePosition;
			touch_detected = true;
		}
		
		if(touch_detected)
		{
			InteractionDetected();
		}
		
		// Listen for arrow keys to switch between effects
		if(m_current_animation_index - 1 >= 0 && Input.GetKeyDown(KeyCode.LeftArrow))
		{
			StartCoroutine(FocusOnAnimation(m_current_animation_index - 1, true));
		}
		if(m_current_animation_index + 1 < m_demo_effects.Length && Input.GetKeyDown(KeyCode.RightArrow))
		{
			StartCoroutine(FocusOnAnimation(m_current_animation_index + 1, true));
		}
		
		if(Input.GetKeyDown(KeyCode.R))
		{
			CurrentAnim.ResetAnimation();
		}
		else if(Input.GetKeyDown(KeyCode.P))
		{
			CurrentAnim.PlayAnimation();
		}
	}
	
	void InteractionDetected()
	{
		int num_touch_effects = CurrentDemo.m_touch_effects.Length;
		if(num_touch_effects > 0)
		{
			if(CurrentDemo.m_touch_effect_pools == null)
			{
				CurrentDemo.m_touch_effect_pools = new EffectManagerPool[num_touch_effects];
				
				for(int pool_idx = 0; pool_idx < CurrentDemo.m_touch_effect_pools.Length; pool_idx++)
				{
					CurrentDemo.m_touch_effect_pools[pool_idx] = new EffectManagerPool(CurrentAnim.transform, CurrentDemo.m_touch_effects[pool_idx]);
				}
			}
			
			int chosen_effect = Random.Range(0,num_touch_effects);
			EffectManager touch_effect = CurrentDemo.m_touch_effect_pools[chosen_effect].GetNextAvailable();
			
			touch_effect.SetText(CurrentDemo.m_touch_strings[CurrentDemo.m_string_pool_idx]);
			CurrentDemo.m_string_pool_idx = CurrentDemo.m_string_pool_idx == CurrentDemo.m_touch_strings.Length - 1 ? 0 : CurrentDemo.m_string_pool_idx + 1;
			
			RaycastHit hit;
        	if (Physics.Raycast(Camera.main.ScreenPointToRay(m_cursor_pos), out hit))
			{
				StartCoroutine(PlayTextFXAnim(touch_effect, hit.point));
			}
		}
	}
	
	IEnumerator PlayTextFXAnim(EffectManager effect, Vector3 position)
	{
		// Position and Play animation
		effect.transform.position = position;
		effect.PlayAnimation();
		
		while(!effect.Playing)
		{
			yield return false;
		}
		
		while(effect.Playing)
		{
			yield return false;
		}
		
		// disable effect object
		effect.enabled = false;
		
#if UNITY_4_0				
		effect.gameObject.SetActive(false);
#else
		effect.gameObject.active = false;
#endif
		
		yield return true;
	}
	
	IEnumerator FocusOnAnimation(int animation_index, bool play_on_complete, bool snap_to_pos = false)
	{
		if(animation_index >= m_demo_effects.Length)
		{
			// requesting anim outside of array bounds
			yield break;
		}
		
		int prev_anim_index = m_current_animation_index;
		
		m_current_animation_index = animation_index;
		
		
		// Reset animation ahead of viewing
		CurrentAnim.ResetAnimation();
		CurrentAnim.SetText(CurrentDemo.m_text);
		
		if(snap_to_pos)
		{
			m_transform.position = -ANIMATION_SPACING * animation_index;
		}
		else
		{
			float timer = 0;
			Vector3 start_pos = m_transform.position;
			Vector3 end_pos = -ANIMATION_SPACING * animation_index;
			
			while(timer < TRANSITION_TIME)
			{
				m_transform.position = Vector3.Lerp(start_pos, end_pos, EffectManager.BackEaseOut(timer / TRANSITION_TIME, 0, 1, 1));
				
				timer += Time.deltaTime;
				yield return false;
			}
			
			m_transform.position = end_pos;
		}
		
		if(play_on_complete)
		{
			CurrentAnim.PlayAnimation();
		}
		
		// Stop playing previous animation which is no longer visible.
		if(prev_anim_index >= 0)
		{
			m_demo_effects[prev_anim_index].m_textfx_animation.ResetAnimation();
		}
	}
	
	void OnGUI()
	{
#if DISPLAY_GUI_CONTROLS
		bool effect_playing = CurrentAnim.Playing;
		
		if(CurrentDemo.m_allow_controls)
		{
			if(GUI.Button(new Rect(10, Screen.height - 60, 80, 50), effect_playing ? "Pause" : "Play"))
			{
				if(!effect_playing)
				{
					if(CurrentAnim.Paused)
					{
						// Unpause animation
						CurrentAnim.Paused = false;
					}
					else
					{
						// Play animation
						Time.timeScale = m_scene_time_scale;
						//					CurrentAnim.SetText(m_text);
						CurrentAnim.PlayAnimation();
					}
				}
				else
				{
					// Pause animation
					CurrentAnim.Paused = true;
				}
			}
			
			if(GUI.Button(new Rect(100, Screen.height - 60, 80, 50), "Stop"))
			{
				CurrentAnim.ResetAnimation();
			}
		}
		
		if(m_current_animation_index - 1 >= 0 && GUI.Button(new Rect(10, Screen.height / 2 - 50, 80, 100), "<"))
		{
			StartCoroutine(FocusOnAnimation(m_current_animation_index - 1, true));
		}
		if(m_current_animation_index + 1 < m_demo_effects.Length && GUI.Button(new Rect(Screen.width - 90, Screen.height / 2 - 50, 80, 100), ">"))
		{
			StartCoroutine(FocusOnAnimation(m_current_animation_index + 1, true));
		}
#endif
	}
}