# sen-aware-game
A demo educational game to support children with Special Educational Needs (SEN)

## SEN-Aware Design Principles

I researched and applied the following principles to create a supportive learning environment:
- **Calm Audio**: Soft, soothing background music to reduce anxiety
- **No Sudden Effects**: Avoided jarring UI effects or sounds that could overwhelm
- **Smooth Transitions**: All interactions feature smooth, gradual transitions
- **No Fail State**: No explicit failure messages; the game encourages continuous engagement without pressure

## Adaptive Difficulty System

The game dynamically adjusts to each player's performance:
- **Three Difficulty Levels**: Easy, Medium, and Hard
- **Starting Point**: The game begins at Medium difficulty
- **Dynamic Adjustment**: After each round, difficulty is updated based on:
  - Time taken to complete successful interactions
  - Number of mistakes made
- **Session-Based Learning**: Initial difficulty is determined at game start using performance metrics from the player's 10 most recent sessions

## Camera Input & Player Attention

I used **MediaPipe** for real-time face detection to enhance engagement and accessibility:
- **Inattention Detection**: When a face is not detected during gameplay, a "Player Inattentive" warning is displayed
- **Analytics Tracking**: Inattention events are recorded for data-driven future improvements and personalized assessments

### Limitations

- The player inattentive warning can give false positive results if playing in dark condition or face too close to the camera.

## Scalability for Platform Growth

The architecture supports expansion:
- **Additional Games**: New educational games can be integrated following the same adaptive framework
- **Refined Assessment**: As we collect more gameplay data, we can fine-tune difficulty adjustment rules
- **Personalized Performance Tracking**: Player profiles enable more sophisticated & data-driven adaptation

## Future Improvements

With more development time, I would focus on:
- Enhanced transition animations between game states
- Additional game modules to diversify learning activities
- Improved visual feedback animations for correct and incorrect responses

## Video Demo:
[Watch Gameplay Demo](https://www.youtube.com/watch?v=KFyeM_ljFf4)

