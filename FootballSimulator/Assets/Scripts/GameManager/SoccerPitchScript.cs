using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerPitchScript : MonoBehaviour
{

    // Team class
    public TeamScript home_team, away_team;

    // Check if any goalkeeper has the ball
    public bool GoalkeeperHasBall;
    public bool GameOn;

    // Goal Script
    public GameObject lhs_goal, rhs_goal;

    // Region class that stores the position of top-left and bottom-right, center, and region id
    // Regions will help players go to a favorable position whe defending or attacking
    // And will position them when kicking off
    public class Region
    {
        public Vector2 center;
        public Vector2 topLeft;
        public Vector2 botRight;

        public int regID;
    }

    public Region PitchRegion = new Region();
    public List<Region> subRegions = new List<Region>();

    // Verical and horizontal regions
    int v_regions = 5;
    int h_regions = 10;

    public GameObject Test;

    private void Awake()
    {
        #region Initialize pitch regions

        // Initialize the main football pitch
        PitchRegion.center = new Vector2(0, 0);
        PitchRegion.topLeft = transform.GetChild(0).transform.position;
        PitchRegion.botRight = transform.GetChild(1).transform.position;
        PitchRegion.regID = -1;

        // Initialize sub-regions
        InitRegions();
        #endregion
    }

    // Start is called before the first frame update
    void Start()
    {
        GameOn = false;

        GoalkeeperHasBall = false;
    }

    // Divide the football pitch into smaller sub regions.
    void InitRegions()
    {
        // 3 regions vertically
        // 6 regions hotizontally
        float reg_height = (PitchRegion.topLeft.y - PitchRegion.botRight.y) / v_regions;
        float reg_width = (PitchRegion.botRight.x - PitchRegion.topLeft.x) / h_regions;

        for (int i = 0; i < v_regions; i++)
        {
            for(int j = 0; j < h_regions; j++)
            {
                Region sub_region = new Region();

                sub_region.topLeft = new Vector2(PitchRegion.topLeft.x + j * reg_width, PitchRegion.topLeft.y - i * reg_height);

                sub_region.botRight = new Vector2(sub_region.topLeft.x + reg_width, sub_region.topLeft.y - reg_height);

                sub_region.center = new Vector2((sub_region.topLeft.x + sub_region.botRight.x) / 2,
                    (sub_region.topLeft.y + sub_region.botRight.y) / 2);

                sub_region.regID = subRegions.Count;

                Instantiate(Test, sub_region.center, new Quaternion(0, 0, 0, 1), this.transform);

                subRegions.Add(sub_region);
            }
        }
    }

    // Update is called once per frame
    //void Update()
    //{
        
    //}
}
