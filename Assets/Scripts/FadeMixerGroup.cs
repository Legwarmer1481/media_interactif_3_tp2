using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

// Je vais l'avouer, je l'ai copié sur Internet. Ce script permet de pouvoir l'appeller avec StartCourotine() me permettant
// de manipuler les volumes des groupe de l'AudioMixer de musique avec une transition le plus fluide que possible.
// Via le gestionnaire de musique, j'appelle la fonction StartFade
// Le premier paramètre est le audioMixer ciblé,
// le deuxième paramètre est de choisi le paramètre de volume de quelle groupe,
// le troisième paramètre est pour la durée de la transition
// Finalement, le dernier est à quelle volume dans le mixer. 1 pour le maximum, 0 pour mettre sur mute.
public static class FadeMixerGroup {
    public static IEnumerator StartFade(AudioMixer audioMixer, string exposedParam, float duration, float targetVolume)
    {
        float currentTime = 0;
        float currentVol;
        audioMixer.GetFloat(exposedParam, out currentVol);
        currentVol = Mathf.Pow(10, currentVol / 20);
        float targetValue = Mathf.Clamp(targetVolume, 0.0001f, 1);
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float newVol = Mathf.Lerp(currentVol, targetValue, currentTime / duration);
            audioMixer.SetFloat(exposedParam, Mathf.Log10(newVol) * 20);
            yield return null;
        }
        yield break;
    }
}