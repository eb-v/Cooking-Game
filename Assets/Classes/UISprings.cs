using System;


//******************************************************************************
// Cached set of motion parameters that can be used to efficiently update
// multiple springs using the same time step, angular frequency and damping
// ratio.
//******************************************************************************
public struct DampedSpringMotionParams
{
    // newPos = posPosCoef*oldPos + posVelCoef*oldVel
    public float PosPosCoef, PosVelCoef;
    // newVel = velPosCoef*oldPos + velVelCoef*oldVel
    public float VelPosCoef, VelVelCoef;
}

public static class UISprings
{
    //******************************************************************************
    // This function will compute the parameters needed to simulate a damped spring
    // over a given period of time.
    // - An angular frequency is given to control how fast the spring oscillates.
    // - A damping ratio is given to control how fast the motion decays.
    //     damping ratio > 1: over damped
    //     damping ratio = 1: critically damped
    //     damping ratio < 1: under damped
    //******************************************************************************
    public static void CalcDampedSpringMotionParams(
        out DampedSpringMotionParams pOutParams,  // motion parameters result
        float deltaTime,                          // time step to advance
        float angularFrequency,                   // angular frequency of motion
        float dampingRatio)                        // damping ratio of motion
    {
        const float epsilon = 0.0001f;

        // Clamp values to legal range
        dampingRatio = Math.Max(dampingRatio, 0f);
        angularFrequency = Math.Max(angularFrequency, 0f);

        pOutParams = new DampedSpringMotionParams();

        if (angularFrequency < epsilon)
        {
            pOutParams.PosPosCoef = 1f;
            pOutParams.PosVelCoef = 0f;
            pOutParams.VelPosCoef = 0f;
            pOutParams.VelVelCoef = 1f;
            return;
        }

        if (dampingRatio > 1.0f + epsilon)
        {
            // Over-damped
            float za = -angularFrequency * dampingRatio;
            float zb = angularFrequency * (float)Math.Sqrt(dampingRatio * dampingRatio - 1.0f);
            float z1 = za - zb;
            float z2 = za + zb;

            float e1 = (float)Math.Exp(z1 * deltaTime);
            float e2 = (float)Math.Exp(z2 * deltaTime);

            float invTwoZb = 1f / (2f * zb);

            float e1OverTwoZb = e1 * invTwoZb;
            float e2OverTwoZb = e2 * invTwoZb;

            float z1e1OverTwoZb = z1 * e1OverTwoZb;
            float z2e2OverTwoZb = z2 * e2OverTwoZb;

            pOutParams.PosPosCoef = e1OverTwoZb * z2 - z2e2OverTwoZb + e2;
            pOutParams.PosVelCoef = -e1OverTwoZb + e2OverTwoZb;

            pOutParams.VelPosCoef = (z1e1OverTwoZb - z2e2OverTwoZb + e2) * z2;
            pOutParams.VelVelCoef = -z1e1OverTwoZb + z2e2OverTwoZb;
        }
        else if (dampingRatio < 1.0f - epsilon)
        {
            // Under-damped
            float omegaZeta = angularFrequency * dampingRatio;
            float alpha = angularFrequency * (float)Math.Sqrt(1f - dampingRatio * dampingRatio);

            float expTerm = (float)Math.Exp(-omegaZeta * deltaTime);
            float cosTerm = (float)Math.Cos(alpha * deltaTime);
            float sinTerm = (float)Math.Sin(alpha * deltaTime);

            float invAlpha = 1f / alpha;

            float expSin = expTerm * sinTerm;
            float expCos = expTerm * cosTerm;
            float expOmegaZetaSinOverAlpha = expTerm * omegaZeta * sinTerm * invAlpha;

            pOutParams.PosPosCoef = expCos + expOmegaZetaSinOverAlpha;
            pOutParams.PosVelCoef = expSin * invAlpha;

            pOutParams.VelPosCoef = -expSin * alpha - omegaZeta * expOmegaZetaSinOverAlpha;
            pOutParams.VelVelCoef = expCos - expOmegaZetaSinOverAlpha;
        }
        else
        {
            // Critically damped
            float expTerm = (float)Math.Exp(-angularFrequency * deltaTime);
            float timeExp = deltaTime * expTerm;
            float timeExpFreq = timeExp * angularFrequency;

            pOutParams.PosPosCoef = timeExpFreq + expTerm;
            pOutParams.PosVelCoef = timeExp;

            pOutParams.VelPosCoef = -angularFrequency * timeExpFreq;
            pOutParams.VelVelCoef = -timeExpFreq + expTerm;
        }
    }


    //******************************************************************************
    // This function will update the supplied position and velocity values over
    // according to the motion parameters.
    //******************************************************************************
    public static void UpdateDampedSpringMotion(
        ref float pos,                        // position value to update
        ref float vel,                        // velocity value to update
        float equilibriumPos,                 // position to approach
        in DampedSpringMotionParams params_)  // motion parameters to use
    {
        float oldPos = pos - equilibriumPos;
        float oldVel = vel;

        pos = oldPos * params_.PosPosCoef + oldVel * params_.PosVelCoef + equilibriumPos;
        vel = oldPos * params_.VelPosCoef + oldVel * params_.VelVelCoef;
    }
}
