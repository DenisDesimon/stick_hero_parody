
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class all_logic : MonoBehaviour
{
    private const float SPEED_CONST = 5f;

    private const float POINT_RIGHT_BORDER = 2.7f;

    private const float POINT_LEFT_BORDER = -2.7f;


    [SerializeField]
    private Transform stick;
        [SerializeField]
    private Transform player;
        [SerializeField]
    private Transform right_Box;
        [SerializeField]
    private Transform left_Box;
        [SerializeField]
    private Transform play_Button;
        [SerializeField]
    private Transform restart_Button;
        [SerializeField]
    private Transform Audio_Button;
        [SerializeField]
    private Transform red_Zone_Cube;
        [SerializeField]
    private AudioClip faling_Down_Audio;
        [SerializeField]
    private AudioClip colide_Stick_Audio;
        [SerializeField]
    private AudioSource increasing_Stick_Audio;
        [SerializeField]
    private Text local_Result;
        [SerializeField]
    private Text global_Result;
       
    private bool is_On_Audio = true;

    private bool is_Game_On = true;

    private int localresult = 0;


    private void Start()
    {
        increasing_Stick_Audio = GetComponent<AudioSource>();
        stick.gameObject.SetActive(false);
        local_Result.gameObject.SetActive(false);
        local_Result.text = "0";
        gameObject.SetActive(false);
        restart_Button.gameObject.SetActive(false);

        if (!PlayerPrefs.HasKey("global_record"))
        {
            PlayerPrefs.SetInt("global_record", 0);
        }
        global_Result.text = "Best result: " + PlayerPrefs.GetInt("global_record").ToString();
    }


    private void OnMouseDown()
    {
        increasing_Stick_Audio.Play();
        if (is_Game_On)
        {
            stick.gameObject.SetActive(true);
        }
    }


    private void OnMouseDrag()
    {

        if (is_Game_On)
        {
            stick.localScale += new Vector3(0, 0.06F, 0);
        }
    }


    void OnMouseUp()
    {
        increasing_Stick_Audio.Stop();
        if (is_Game_On)
        {
            StartCoroutine("Rotating_stick_Moving_player_Moving_Boxes");
        }
        is_Game_On = false;


    }


    public void On_Press_Switch_Audio()
    {
        if (is_On_Audio)
        {
            AudioListener.volume = 0;
            is_On_Audio = false;
        }
        else
        {
            AudioListener.volume = 1;
            is_On_Audio = true;
        }
    }



    public void On_Press_Button_Play()
    {
        Audio_Button.gameObject.SetActive(false);
        play_Button.gameObject.SetActive(false);
        gameObject.SetActive(true);
        local_Result.gameObject.SetActive(true);
        global_Result.gameObject.SetActive(false);
        StartCoroutine("On_Press_Play_Button_Couratine");
        
    }

    public void On_Press_Button_Restart()
    {
        Audio_Button.gameObject.SetActive(false);
        global_Result.gameObject.SetActive(false);
        restart_Button.gameObject.SetActive(false);
        localresult = 0;
        local_Result.text = localresult.ToString();
        StartCoroutine("On_Press_Restart_Button_Couratine");
        StartCoroutine("On_Press_Play_Button_Couratine");
       
    }

    IEnumerator On_Press_Restart_Button_Couratine()
    {
        player.position = new Vector3(POINT_LEFT_BORDER - player.localScale.x, -1f);

        while (player.position.x + player.localScale.x / 2 + 0.15f 
            < left_Box.position.x + left_Box.localScale.x / 2)
        {

            player.position += new Vector3(SPEED_CONST * Time.deltaTime, 0);
            yield return null;
        }

        stick.localScale = new Vector3(1, 1, 1);

        stick.position = new Vector3(player.position.x + player.localScale.x / 2, stick.position.y);
        stick.Rotate(new Vector3(0, 0, 90));

        is_Game_On = true;
    }


    IEnumerator On_Press_Play_Button_Couratine()
    {
        while (left_Box.position.x > POINT_LEFT_BORDER)
        {
            player.position -= new Vector3(SPEED_CONST * 2 * Time.deltaTime, 0);            
            left_Box.position -= new Vector3(SPEED_CONST * 2 * Time.deltaTime, 0);
            stick.position -= new Vector3(SPEED_CONST * 2 * Time.deltaTime, 0);
            if(player.position.x + player.localScale.x / 2 + 0.15f 
                < left_Box.position.x + left_Box.localScale.x / 2)
                player.position += new Vector3(SPEED_CONST * Time.deltaTime, 0);
            yield return null;
        }
        float m = Make_New_Box();
        while (m < right_Box.position.x)
        {
            red_Zone_Cube.position -= new Vector3(SPEED_CONST * Time.deltaTime * 2, 0);
            right_Box.position -= new Vector3(SPEED_CONST * Time.deltaTime * 2, 0);
            
            yield return null;
        }


    }

    IEnumerator Rotating_stick_Moving_player_Moving_Boxes()
    {
        while (stick.eulerAngles.z != 270)
        {
            stick.Rotate(new Vector3(0, 0, -3));

            yield return null;
        }
        AudioSource.PlayClipAtPoint(colide_Stick_Audio, new Vector3(0, 0, 0));

        if (! Check_Local_Result())
        {
            while (player.position.x - player.localScale.x / 2  < stick.localScale.y + stick.position.x)
            {

                player.position += new Vector3(SPEED_CONST * Time.deltaTime, 0);
                yield return null;
            }
            AudioSource.PlayClipAtPoint(faling_Down_Audio, new Vector3(0,0,0));
            
           
            while (player.position.y - player.localScale.y / 2 > -6f)
            {

                player.position += new Vector3(0, (-SPEED_CONST) * 2 * Time.deltaTime);
                yield return null;
            }
            Audio_Button.gameObject.SetActive(true);
            global_Result.gameObject.SetActive(true);
            restart_Button.gameObject.SetActive(true);
        }
        else
        {
            while (player.position.x + player.localScale.x / 2 + 0.15f 
                < right_Box.position.x + right_Box.localScale.x / 2)
            {

                player.position += new Vector3(SPEED_CONST * Time.deltaTime, 0);      
                yield return null;
            }
            local_Result.text = localresult.ToString();
            while (right_Box.position.x > POINT_LEFT_BORDER)
            {
                player.position -= new Vector3(SPEED_CONST * Time.deltaTime, 0);
                right_Box.position -= new Vector3(SPEED_CONST * Time.deltaTime, 0);
                left_Box.position -= new Vector3(SPEED_CONST * Time.deltaTime, 0);
                red_Zone_Cube.position -= new Vector3(SPEED_CONST * Time.deltaTime , 0);
                stick.position -= new Vector3(SPEED_CONST * Time.deltaTime, 0);
                yield return null;
               
            }
            float m = Make_New_Box(true,true);
            while ( m < right_Box.position.x )
            {
                red_Zone_Cube.position -= new Vector3(SPEED_CONST * Time.deltaTime * 2, 0);
                right_Box.position -= new Vector3(SPEED_CONST * Time.deltaTime * 2, 0);
                yield return null;
            }

            is_Game_On = true;
        }
    }


    float Make_New_Box(bool should_switch_boxes = false, bool should_rotate_stick = false)
    {
        if (should_switch_boxes)
        {
            Transform t;
            t = right_Box;
            right_Box = left_Box;
            left_Box = t;
        }
        right_Box.position = new Vector3(POINT_RIGHT_BORDER+right_Box.localScale.x, right_Box.position.y, right_Box.position.z);

        right_Box.localScale = new Vector3(Random.Range(0.4f, 1.8f), right_Box.localScale.y, right_Box.localScale.z);

        stick.localScale = new Vector3(1, 1, 1);



        stick.position = new Vector3(player.position.x + player.localScale.x / 2, stick.position.y);
        if (should_rotate_stick)
        {
            stick.Rotate(new Vector3(0, 0, 90));
        }
        stick.gameObject.SetActive(false);

        red_Zone_Cube.localScale = new Vector3(right_Box.localScale.x / 5, red_Zone_Cube.localScale.y);
        red_Zone_Cube.position = new Vector3(right_Box.position.x, right_Box.position.y + right_Box.localScale.y / 2 - red_Zone_Cube.localScale.y / 2, (-2f));


        return Random.Range(-1f, 2f);
    }


    bool Check_Local_Result()
    {
        if (((stick.localScale.y + stick.position.x)
            > (red_Zone_Cube.position.x - red_Zone_Cube.localScale.x / 2))
            && (stick.localScale.y + stick.position.x
            < (red_Zone_Cube.position.x + red_Zone_Cube.localScale.x / 2)))
        {
            localresult += 2;
            if (localresult > PlayerPrefs.GetInt("global_record"))
                PlayerPrefs.SetInt("global_record", localresult);
            return true;
        }
        else if (((stick.localScale.y + stick.position.x)
            < (right_Box.position.x - right_Box.localScale.x / 2))
            || (stick.localScale.y + stick.position.x >
            (right_Box.position.x + right_Box.localScale.x / 2)))
            return false;
        else
        {
            localresult++;
            if (localresult > PlayerPrefs.GetInt("global_record"))
                PlayerPrefs.SetInt("global_record", localresult);
            return true;
        }
    }


}
