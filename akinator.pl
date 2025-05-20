% Allow dynamic addition of new facts
:- dynamic game/9.

% Structure: game(Name, Genre, Platform, Setting, CameraType, Multiplayer, GraphicsStyle, Difficulty, AAA).

game('The Witcher 3', 1, 1, 1, 1, 2, 1, 2, 1).
game('Dark Souls', 1, 1, 2, 1, 2, 2, 3, 1).
game('Among Us', 3, 2, 3, 3, 1, 3, 1, 0).
game('Stardew Valley', 4, 2, 4, 2, 2, 4, 1, 0).
game('Cyberpunk 2077', 1, 1, 5, 1, 2, 1, 2, 1).
game('Minecraft', 5, 2, 4, 2, 2, 3, 1, 0).
game('League of Legends', 2, 1, 3, 4, 1, 2, 2, 1).
game('Tetris', 6, 3, 6, 2, 2, 5, 1, 0).
game('Red Dead Redemption 2', 1, 1, 2, 1, 2, 1, 2, 1).

% Questions
ask_question(1, 'What is the genre of the game?', ['1. RPG', '2. MOBA', '3. Party', '4. Simulator', '5. Sandbox', '6. Arcade']).
ask_question(2, 'Which platform?', ['1. PC/Console', '2. Mobile', '3. Arcade machine']).
ask_question(3, 'What is the setting?', ['1. Fantasy', '2. Dark fantasy', '3. Space', '4. Life/Farm', '5. Cyberpunk', '6. Abstract']).
ask_question(4, 'Camera type?', ['1. Third-person', '2. Top-down', '3. 2D', '4. Isometric']).
ask_question(5, 'Is it multiplayer?', ['1. Yes', '2. No']).
ask_question(6, 'Graphics style?', ['1. Photorealistic', '2. Stylized', '3. Pixel', '4. 2D cartoon', '5. Abstract']).
ask_question(7, 'Difficulty?', ['1. Easy', '2. Medium', '3. Hard']).
ask_question(8, 'Is it a AAA title?', ['1. Yes', '2. No']).

% Ask the user a question and get the answer
ask_user(Q, Answer) :-
    ask_question(Q, Question, Options),
    format('~w ~w~n', [Question, Options]),
    read(Answer).

% Try to guess the game based on provided answers (prefix matching)
guess_game(Answers) :-
    game(Game, G1, G2, G3, G4, G5, G6, G7, G8),
    prefix(Answers, [G1, G2, G3, G4, G5, G6, G7, G8]),
    format('I guess the game is: ~w~n', [Game]).

% Check if one list is a prefix of another
prefix([], _).
prefix([H|T], [H|T2]) :- prefix(T, T2).

% Add a new game to the database
add_game(Name, G1, G2, G3, G4, G5, G6, G7, G8) :-
    atom(Name),
    assertz(game(Name, G1, G2, G3, G4, G5, G6, G7, G8)),
    format('Game ~w has been added to the database!~n', [Name]).

% Start the game session
start_game :-
    ask_user(1, A1),
    ask_user(2, A2),
    ask_user(3, A3),
    ask_user(4, A4),
    ask_user(5, A5),
    ask_user(6, A6),
    ask_user(7, A7),
    ask_user(8, A8),
    Answers = [A1, A2, A3, A4, A5, A6, A7, A8],
    (   guess_game(Answers)
    ->  true
    ;   format('Could not guess the game! Do you want to add a new one? (yes/no)~n'),
        read(Response),
        (   Response = 'yes'
        ->  format('Enter the name of the game (in quotes): '),
            read(Name),
            add_game(Name, A1, A2, A3, A4, A5, A6, A7, A8)
        ;   format('Thanks for playing!~n')
        )
    ).
