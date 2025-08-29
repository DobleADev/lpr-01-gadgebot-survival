using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GadgebotSurvivalVFXGadgebotTransition : MonoBehaviour
{
    [SerializeField] float transitionHeight = 5;
    [SerializeField] float minLightScale = 0.3f;
    [SerializeField] float maxLightScale = 0.9f;
    [SerializeField] Transform transitionLight;
    [SerializeField] Transform transitionParticles;
    [SerializeField] float transitionDuration = 1;
    [SerializeField] float appearScaleEndDuration = 0.2f;
    [SerializeField] float appearScaleStartDuration = 0.2f;
    Coroutine transitionRoutine;

    public void SnapToGadgebot(Gadgebot gadgebot)
    {
        transform.position = gadgebot.transform.position;
    }

    public void DoAppearTransition()
    {
        gameObject.SetActive(true);
        transitionRoutine = StartCoroutine(AppearTransitionProcess());

    }

    public void DoDissappearTransition()
    {
        gameObject.SetActive(true);
        transitionRoutine = StartCoroutine(DissappearTransitionProcess());
    }

    IEnumerator AppearTransitionProcess()
    {
        Vector2 startPosition = (Vector2)transform.position + new Vector2(0, transitionHeight);
        Vector2 endPosition = transform.position;
        transitionLight.localScale = minLightScale * Vector3.one;
        transitionParticles.gameObject.SetActive(true);

        float t = 0, deltaDuration = 1 / transitionDuration;
        while (t < 1)
        {
            transform.position = Vector2.Lerp(startPosition, endPosition, t);
            t += deltaDuration * Time.deltaTime;
            yield return null;
        }
        transform.position = endPosition;

        t = 0;
        deltaDuration = 1 / appearScaleEndDuration;
        while (t < 1)
        {
            transitionLight.localScale = Mathf.Lerp(minLightScale, maxLightScale, t) * Vector3.one;
            t += deltaDuration * Time.deltaTime;
            yield return null;
        }
        transitionLight.localScale = maxLightScale * Vector3.one;

        gameObject.SetActive(false);
    }
    
    IEnumerator DissappearTransitionProcess()
    {
        Vector2 startPosition = transform.position;
        Vector2 endPosition = (Vector2)transform.position + new Vector2(0, transitionHeight);
        float startLightScale = 0.1f;
        transitionLight.localScale = startLightScale * Vector3.one;
        transitionParticles.gameObject.SetActive(false);

        float t = 0, deltaDuration = 1 / appearScaleStartDuration;
        while (t < 1)
        {
            transitionLight.localScale = Mathf.Lerp(startLightScale, maxLightScale, t) * Vector3.one;
            t += deltaDuration * Time.deltaTime;
            yield return null;
        }
        transitionLight.localScale = maxLightScale * Vector3.one;

        t = 0;
        deltaDuration = 1 / transitionDuration;
        while (t < 1)
        {
            transform.position = Vector2.Lerp(startPosition, endPosition, t);
            transitionLight.localScale = Mathf.Lerp(maxLightScale, 0, t) * Vector3.one;
            t += deltaDuration * Time.deltaTime;
            yield return null;
        }
        transform.position = endPosition;
        transitionLight.localScale = 0 * Vector3.one;

        gameObject.SetActive(false);
    }
}
