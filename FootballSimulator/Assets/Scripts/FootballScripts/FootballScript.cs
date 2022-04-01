using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FootballScript : MonoBehaviour
{

    //float TestCounter = 0f;

    public GameObject ScoreLeft, ScoreRight;
    public GameObject Winpanel, LosePanel;

    public TeamScript homeTeam, awayTeam;

    CircleCollider2D BallCollider;

    public GameObject Z_AxisManager;

    Rigidbody2D BallRigidbody;

    public float desiredHeight;

    // Start is called before the first frame update
    void Start()
    {
        BallCollider = GetComponent<CircleCollider2D>();

        BallRigidbody = GetComponent<Rigidbody2D>();

        Z_AxisManager = GameObject.Find("ZAxisManager");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Gravitation();

        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3((transform.position.z) + 0.15f, (transform.position.z) + 0.15f, 1), 10 * Time.deltaTime);

        ScoreGoal();

        OutField();
    }


    // Handles the ball when it is in the air.
    void Gravitation()
    {
        float z_pos = Z_AxisManager.transform.position.y / 5f;
        transform.position = new Vector3(transform.position.x, transform.position.y, z_pos);
    }

    // Calculates when the ball is out of the field.
    void OutField()
    {
        if (BallCollider.IsTouchingLayers(LayerMask.GetMask("OutField")) && !BallCollider.IsTouchingLayers(LayerMask.GetMask("FieldLine")) && !BallCollider.IsTouchingLayers(LayerMask.GetMask("GoalDetection")))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    // Calculates when the ball is behind the goal line.
    void ScoreGoal()
    {
        if (GameObject.FindGameObjectWithTag("Pitch").GetComponent<SoccerPitchScript>().GameOn == true &&
            BallCollider.IsTouchingLayers(LayerMask.GetMask("GoalDetection")) &&
            !BallCollider.IsTouchingLayers(LayerMask.GetMask("FieldLine")))
        {
            GameObject.Find("MatchManager").GetComponent<MatchManager>().goalsCounter++;

            GameObject.FindGameObjectWithTag("Pitch").GetComponent<SoccerPitchScript>().GameOn = false;

            homeTeam.ChangeState(homeTeam.state_Defending);

            awayTeam.ChangeState(awayTeam.state_Defending);

            homeTeam.ChangeState(homeTeam.state_PrepareForKickOff);

            awayTeam.ChangeState(awayTeam.state_PrepareForKickOff);

            homeTeam.m_HasBall = false;

            awayTeam.m_HasBall = false;

            IncreaseScore();
        }
    }

    void IncreaseScore()
    {
        bool ScoringTeamLeft = transform.position.x > 0;

        GameObject[] teams = GameObject.FindGameObjectsWithTag("Team");

        Image[] scoreUI;

        if (ScoringTeamLeft)
        {
            scoreUI = ScoreLeft.GetComponentsInChildren<Image>();
        }
        else
        {
            scoreUI = ScoreRight.GetComponentsInChildren<Image>();
        }


        for(int i = 0; i < scoreUI.Length; i++)
        {
            if(scoreUI[i].color != Color.green)
            {
                scoreUI[i].color = Color.green;

                if(i == scoreUI.Length - 1)
                {
                    if(ScoringTeamLeft)
                    {
                        StartCoroutine(EndGame(true));
                    }
                    else
                    {
                        StartCoroutine(EndGame(false));
                    }
                    return;
                }

                break;
            }
        }

        Invoke("RepositionBall", 2.0f);
    }

    IEnumerator EndGame(bool won)
    {
        yield return new WaitForSeconds(3.0f);

        if(won)
        {
            Winpanel.SetActive(true);
        }
        else
        {
            LosePanel.SetActive(true);
        }
    }

    void RepositionBall()
    {
        transform.position = Vector3.zero;

        BallRigidbody.angularVelocity = 0.0f;

        BallRigidbody.velocity = Vector2.zero;
    }

    // Kick the ball in a direction, by a given power and height
    public void KickBall(Vector2 kickDirection, float kickPower, float kickHeight)
    {
        BallRigidbody.AddForce(kickDirection * kickPower / 20, ForceMode2D.Impulse);

        Z_AxisManager.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, kickHeight), ForceMode2D.Impulse);
    }

    public void KickBall(Vector2 kickDirection, float kickPower)
    {
        BallRigidbody.AddForce(kickDirection * kickPower / 20, ForceMode2D.Impulse);
    }

    // Calculate the error angle when kicking the ball
    public float CalculateErrorAngle(float Attribute, float targetDistance)
    {

        float ErrorAngle = Mathf.Asin(Mathf.Pow(UnityEngine.Random.Range(0f, 1f), Attribute / (targetDistance))) * Mathf.Rad2Deg;

        // The eror angle should be max. 60•
        ErrorAngle = Mathf.Clamp(ErrorAngle, 0.0f, 60.0f);

        if (UnityEngine.Random.Range(0, 2) == 1)
        {
            ErrorAngle *= -1;
        }

        return ErrorAngle;
    }

    public float TimeToCoverDistance(Vector2 A_pos, Vector2 B_pos, float kickPower)
    {
        // Programming Game AI by Example - Mat Buckland

        float time = 0;

        kickPower /= 20.0f;

        // Starting position of the ball
        Vector2 pPos = A_pos;

        // Initial distance
        float startDistance = Vector2.Distance(B_pos, A_pos);

        // Drag multiplier (friction)
        float rDrag = Mathf.Clamp01(1.0f - (BallRigidbody.drag * Time.fixedDeltaTime));

        // Direction of the ball
        Vector2 dir = B_pos - pPos;

        // How much velocity is added per frame
        Vector2 velocityPerFrame = dir.normalized * Time.fixedDeltaTime * kickPower;

        if (velocityPerFrame != Vector2.zero)
        {
            // How many frames are going to pass in the given time
            while(Vector2.Distance(pPos, B_pos) > 0.01f)
            {
                float dis1 = Vector2.Distance(pPos, B_pos);

                velocityPerFrame *= rDrag;
                pPos += velocityPerFrame;

                float dis2 = Vector2.Distance(pPos, B_pos);

                // If the ball is going to stop before reaching the destination
                if (velocityPerFrame.magnitude < 0.005f)
                {
                    return -1;
                }

                // We might not catch when the ball is really close to the desired target
                if(dis2 > dis1)
                {
                    return time;
                }

                time += Time.fixedDeltaTime;
            }
        }

        return time;
    }

    public Vector2 FuturePosition(float time)
    {
        // Programming Game AI by Example - Mat Buckland
        // https://answers.unity.com/questions/1087568/3d-trajectory-prediction.html

        // Starting position of the ball
        Vector2 pPos = BallRigidbody.position;

        // Drag multiplier (friction)
        float rDrag = Mathf.Clamp01(1.0f - (BallRigidbody.drag * Time.fixedDeltaTime));

        // How much velocity is added per frame
        Vector2 velocityPerFrame = BallRigidbody.velocity;
        
        // How many frames are going to pass in the given time
        for(int i = 0; i < time/Time.fixedDeltaTime; i++)
        {
            velocityPerFrame *= rDrag;
            pPos += (velocityPerFrame * Time.fixedDeltaTime);
        }

        return pPos;
    }
}
