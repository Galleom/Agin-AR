using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyAR;

/* This script fixes the model shake behavior on EasyAr v2. You should 
use this instead of ImageTargetBehaviour on your target. Just delete the
ImageTargetBehaviour component and add this one 
It is a fixed and improved version of the one posted on this thread: 
https://answers.easyar.com/296/how-to-reduce-ar-camera-shaking
This can still be improved a lot, but it is good enough for me
*/

public class ImageTargetBehaviourLessShake : ImageTargetBehaviour {

    private float lastX;
    private float lastRX;
    private float lastY;
    private float lastRY;
    private float lastZ;
    private float lastRZ;

    // Fine-tune this values according to your own project
    public float rotMin = 3f;
    public float rotMax = 6f;
    public float trasMin = 0.06f;
    public float trasMax = 0.13f;
    public float lerpT = 0.2f;

    protected override void Update() {
        base.Update();

        float myrx = 0;
        myrx = this.transform.localEulerAngles.x;
        while (myrx >= 360)
        {
            myrx -= 360;
        }
        while (myrx <= -360)
        {
            myrx += 360;
        }
        while (myrx > 270 && 360 - myrx >= 0)
            myrx = -(360 - myrx);
        float myry = 0;
        myry = this.transform.localEulerAngles.y;
        while (myry >= 360)
        {
            myry -= 360;
        }
        while (myry <= -360)
        {
            myry += 360;
        }
    
        while (myry > 270 && 360 - myry >= 0)
            myry = -(360 - myry);

        float myrz = 0;
        myrz = this.transform.localEulerAngles.z;
        while (myrz >= 360)
        {
            myrz -= 360;
        }

        while (myrz <= -360)
        {
            myrz += 360;
        }

        while (myrz > 270 && 360 - myrz >= 0)
            myrz = -(360 - myrz);
            
        if (
            (
                (Mathf.Abs(this.transform.position.x - lastX) > trasMin || Mathf.Abs(this.transform.position.y - lastY) > trasMin || Mathf.Abs(this.transform.position.z - lastZ) > trasMin) &&
                (Mathf.Abs(this.transform.position.x - lastX) < trasMax || Mathf.Abs(this.transform.position.y - lastY) < trasMax || Mathf.Abs(this.transform.position.z - lastZ) < trasMax)) ||
            ( 
                (Mathf.Abs(myrx - lastRX) > rotMin && Mathf.Abs(myry - lastRY) > rotMin && Mathf.Abs(myrz - lastRZ) > rotMin) && 
                (Mathf.Abs(myrx - lastRX) < rotMax || Mathf.Abs(myry - lastRY) < rotMax || Mathf.Abs(myrz - lastRZ) < rotMax)
            )
        ) {
            lastX = Mathf.Lerp(lastX, this.transform.position.x, lerpT);
            lastY = Mathf.Lerp(lastY, this.transform.position.y, lerpT);
            lastZ = Mathf.Lerp(lastZ, this.transform.position.z, lerpT);
            lastRX = Mathf.Lerp(lastRX, myrx, lerpT);
            lastRY = Mathf.Lerp(lastRY, myry, lerpT);
            lastRZ = Mathf.Lerp(lastRZ, myrz, lerpT);
        }
        
        this.transform.rotation = Quaternion.Euler(lastRX, lastRY, lastRZ);
        this.transform.position = new Vector3(lastX, lastY, lastZ);
        
           
    }
}