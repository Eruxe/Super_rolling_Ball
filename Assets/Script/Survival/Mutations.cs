using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;


public class Mutations : MonoBehaviour
{

    public delegate float TwoVariablesFunction(float x1, float x2);

    public static TwoVariablesFunction MutationSelectorY(Random Generator, int chaosFrequencyY, ref float sizex)
    {
        //VARIABLES POUR LES MUTATIONS
        float copyx = sizex;
        float intensity;
        float frequency = (float)Math.PI * 2;
        int negative;

        if (Generator.Next(100) > chaosFrequencyY)
        {
            int select = Generator.Next(5);
            negative = Generator.Next(2);
            //SELECTEUR MANUEL CI DESSOUS
            //select = 1;

            switch (select)
            {
                case 0: //MONTE ET DESCENTE
                    if (negative > 0) copyx = -sizex;
                    return (pas, z) => Mathf.Lerp(0, copyx * 0.5f, pas);
                case 1: //VAGUE BASSE
                    sizex += Generator.Next(8, 15);
                    intensity = (float)(Generator.Next(10, 25)) / 10;
                    return (pas, z) => (intensity * Mathf.Cos(pas * frequency) - intensity);
                case 2: //VAGUE HAUTE
                    sizex += Generator.Next(6, 12);
                    intensity = (float)(Generator.Next(8, 15)) / 10;
                    return (pas, z) => (intensity * Mathf.Cos(pas * frequency + (float)Math.PI) + intensity);
                case 3: //VAGUE MONTE
                    intensity = (float)(Generator.Next(6, 9)) / 10;
                    frequency = 8;
                    return (pas, z) => Mathf.Lerp(0, copyx * 0.3f + (intensity * Mathf.Sin(pas * frequency)), pas);
                case 4: //VAGUE DESCENTE
                    sizex += Generator.Next(3, 7);
                    copyx = sizex;
                    intensity = (float)(Generator.Next(7, 17)) / 10;
                    return (pas, z) => Mathf.Lerp(0, -copyx * 0.5f + (intensity * Mathf.Sin(pas * frequency)), pas);
            }
        }
        return (pas, z) => Mathf.Lerp(0, 0, pas);
    }

    public static TwoVariablesFunction MutationSelectorWidth(Random Generator, int chaosFrequencyZ, float sizez)
    {
        if (Generator.Next(100) > chaosFrequencyZ)
        {
            int nextSizeZ = Generator.Next(3, 7);
            return (pas, z) => Mathf.Lerp(0, nextSizeZ - sizez, pas);
        }
        return (pas, z) => Mathf.Lerp(0, 0, pas);
    }

    public static TwoVariablesFunction MutationSelectorZ(Random Generator, int chaosFrequencyZ, ref float sizex)
    {
        //VAR POUR MUTATION
        float copyx = sizex;
        float angle;
        float intensity;
        float frequency;
        int negative;

        if (Generator.Next(100) > chaosFrequencyZ)
        {
            int select = Generator.Next(3);
            negative = Generator.Next(2);
            //SELECTEUR MANUEL CI DESSOUS
            //select = 2;

            switch (select)
            {
                case 0: //DIAGONALE DROITE
                    angle = (float)(Generator.Next(3, 12)) / 10;
                    if (negative > 0) copyx = -sizex;
                    return (pas, z) => Mathf.Lerp(0, copyx * angle, pas);
                case 1: //VAGUES DROITE
                    sizex += Generator.Next(5, 10);
                    copyx = sizex;
                    intensity = (float)(Generator.Next(15, 25)) / 10;
                    frequency = (float)(Generator.Next(5, 9));
                    if (negative > 0) intensity = -intensity;
                    return (pas, z) => (intensity * Mathf.Cos(pas * frequency) - intensity);
                case 2: //VAGUE DIAGONALE
                    angle = (float)(Generator.Next(3, 8)) / 10;
                    sizex += Generator.Next(3, 8);
                    copyx = sizex;
                    if (negative > 0) copyx = -sizex;
                    intensity = (float)(Generator.Next(20, 35)) / 10;
                    frequency = (float)(Generator.Next(5, 9));
                    return (pas, z) => Mathf.Lerp(0, copyx * angle + (intensity * Mathf.Sin(pas * frequency)), pas);
            }
        }
        return (pas, z) => Mathf.Lerp(0, 0, pas);
    }

}
