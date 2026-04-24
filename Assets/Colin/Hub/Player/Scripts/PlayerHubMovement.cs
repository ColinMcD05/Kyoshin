using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHubMovement : MonoBehaviour
{

    public int moveSpeed;
    public int rotationSpeed;
    public Vector3 direction;
    [SerializeField] Transform orientation;
    [SerializeField] Transform playerVisual; // child of this object

    AudioSource music;
    Songs songs;


    void Start()
    {
        music = GameObject.Find("Audio").transform.Find("Music").GetComponent<AudioSource>();
        songs = GameObject.Find("GameManager").GetComponent<Songs>();

        foreach (Songs.SongData song in songs.songs)
        {
            if (song.levelName == SceneManager.GetActiveScene().name)
            {
                music.PlayOneShot(song.song);
            }
        }
    }


    void Update()
    {
        Movement();
    }

    public void Movement()
    {
        transform.Translate(direction * moveSpeed * Time.deltaTime, Space.Self);
        // rotate only visual, not root
        if (direction.sqrMagnitude > 0.001f) // if the direction is not 0, then rotate the visual
        {        
        // get the facing directio
        Quaternion moveFacing = Quaternion.LookRotation(direction, Vector3.up); // using Quaternion.LookRotation to get the facing direction
        // compensate model/prefab forward mismatch
        // // get the offset between the model and the parent
        Quaternion modelOffset = Quaternion.AngleAxis(90f, Vector3.up); // Because the model is a different direction then the parent
        Quaternion target = moveFacing * modelOffset; //multiply the two quaternions to get the target rotation
        // // rotate the child to the target rotation
        // playerVisual is the child of the parent, Quaternion.RotateTowards is used to rotate the child to the target rotationm, playerVisual.rotation is the current rotation of the child, target is the target rotation, rotationSpeed is the speed of the rotation
        playerVisual.rotation = Quaternion.RotateTowards(playerVisual.rotation,target, rotationSpeed * Time.deltaTime );
        }
        
    }

    public void SetDirection(Vector2 movement)
    {
        Vector3 xInput = movement.x * orientation.right;
        Vector3 yInput = movement.y * orientation.forward;
        direction = xInput + yInput;

      
}
}
