using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using bsc;

public class Player : MonoBehaviour
{
    [SerializeField]
    private Transform orientation;
    [SerializeField]
    private SuperPlayerController body;
    [SerializeField]
    private ThirdPersonCamera _camera;
    [SerializeField]
    private Transform viewModelHolder;

    private Slider[] crosshairSegments;

    private readonly Counter recoil = new();
    private float maxRecoil = 4;

    private readonly Counter firedelay = new();
    private float fireinterval = 10;

    private GameObject viewModel = null;
    private Animator viewModelAnimator = null;
    private GameObject viewModel2 = null;
    private Animator viewModelAnimator2 = null;

    private bool isReadyForAction => readyForActionTime > 0;
    private float readyForActionTime;

    private readonly float fixedDeltaTime = 1f / 60f;

    private int alternator = 1;

    public float AttackSpeed { get; private set; } = 1;

    public float DeltaTime { get; private set; }

    private void Awake()
    {
        var viewModels = GameObject.FindGameObjectsWithTag("VIEWMODEL");
        viewModel = viewModels[0];
        viewModelAnimator = viewModel.GetComponent<Animator>();
        viewModel2 = viewModels[1];
        viewModelAnimator2 = viewModel2.GetComponent<Animator>();

        recoil.Reconfigure(maxRecoil);
        recoil.rate = 0.2f;
        firedelay.Reconfigure(fireinterval / AttackSpeed);

        crosshairSegments = new Slider[4] {
            GameObject.FindGameObjectWithTag("CrosshairW").GetComponent<Slider>(),
            GameObject.FindGameObjectWithTag("CrosshairE").GetComponent<Slider>(),
            GameObject.FindGameObjectWithTag("CrosshairN").GetComponent<Slider>(),
            GameObject.FindGameObjectWithTag("CrosshairS").GetComponent<Slider>()
        };
    }

    private void Update()
    {
        DeltaTime = Time.deltaTime;
        UpdateMouseLook();
    }

    private void UpdateMouseLook()
    {
        if(isReadyForAction)
        {
            viewModelHolder.localEulerAngles = Vector3.right * _camera.Pitch;
        }
        else
        {
            viewModelHolder.forward = Vector3.Slerp(viewModelHolder.forward, body.transform.forward, 10 * DeltaTime);
        }
    }

    private void FixedUpdate()
    {
        DeltaTime = Time.fixedDeltaTime;
        SingleUpdate();
    }

    private void SingleUpdate()
    {
        UpdateTickers();
        UpdateAttacks();
        UpdateCrosshair();
        UpdateModel();
    }

    private void UpdateTickers()
    {
        recoil.Update(DeltaTime);
        firedelay.Update(DeltaTime);
    }

    private void UpdateAttacks()
    {
        if(readyForActionTime > 0)
        {
            readyForActionTime = Utils.Approach(readyForActionTime, 0, 1 * DeltaTime);
        }

        if(Input.GetMouseButton(0) && firedelay.IsDone)
        {
            firedelay.Reset();
            recoil.value = Mathf.Min(recoil.value + 2.5f, maxRecoil);

            readyForActionTime = 3;

            viewModelAnimator.SetFloat("atkSpd", AttackSpeed);
            viewModelAnimator2.SetFloat("atkSpd", AttackSpeed);

            if(alternator == 1)
            {
                viewModelAnimator.SetBool("firing", true);
                viewModelAnimator2.SetBool("firing", false);
            }
            else
            {
                viewModelAnimator.SetBool("firing", false);
                viewModelAnimator2.SetBool("firing", true);
            }

            alternator = -alternator;

            SFX.Play($"mando_M1_{Random.Range(1, 12)}", 0.1f);
        }
        else
        {
            viewModelAnimator.SetBool("firing", false);
            viewModelAnimator2.SetBool("firing", false);
        }
    }

    private void UpdateCrosshair()
    {
        foreach(var slider in crosshairSegments)
        {
            try
            {
                slider.value = recoil.value / maxRecoil;
            }
            catch(System.Exception e)
            {
                Debug.LogException(e, gameObject);
                return;
            }
        }
    }

    private void UpdateModel()
    {
        if(isReadyForAction)
        {
            body.transform.forward = Vector3.Slerp(body.transform.forward, orientation.forward, 12 * DeltaTime);
        }
    }
}
