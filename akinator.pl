:- dynamic game/9.
:- consult('games_db.pl').

% Вопросы
ask_question(1, 'Is the game an RPG?', ['y', 'n']).
ask_question(2, 'Is it primarily on PC or consoles?', ['y', 'n']).
ask_question(3, 'Is the setting fantasy?', ['y', 'n']).
ask_question(4, 'Is the camera third-person?', ['y', 'n']).
ask_question(5, 'Does it have multiplayer?', ['y', 'n']).
ask_question(6, 'Is the graphics style photorealistic?', ['y', 'n']).
ask_question(7, 'Is it considered difficult?', ['y', 'n']).
ask_question(8, 'Is it a AAA title?', ['y', 'n']).

% Проверка префикса (совпадение списков ответов)
prefix([], _).
prefix([H|T], [H2|T2]) :-
    H == H2,
    prefix(T, T2).

% Находим игры, подходящие под текущие ответы
possible_games(Answers, Games) :-
    findall(Game, (game(Game, G1,G2,G3,G4,G5,G6,G7,G8),
                   prefix(Answers, [G1,G2,G3,G4,G5,G6,G7,G8])), Games).

% Возвращаем следующий вопрос с вариантами
next_question(Answers, Question, Options) :-
    length(Answers, Len),
    Len < 8,
    NextQ is Len + 1,
    ask_question(NextQ, Question, Options).

% Основной предикат шага: возвращает либо игру, либо вопрос, либо сообщение
next_step(Answers, Response, Done) :-
    possible_games(Answers, Games),
    ( Games = [] ->
        Response = no_match,
        Done = true
    ; Games = [OnlyGame] ->
        Response = game(OnlyGame),
        Done = true
    ; next_question(Answers, Question, Options) ->
        Response = question(Question, Options),
        Done = false
    ; Response = multiple_matches,
      Done = true
    ).

% Добавление игры и сохранение базы
add_and_save_game(Name, G1,G2,G3,G4,G5,G6,G7,G8) :-
    atom(Name),
    assertz(game(Name, G1,G2,G3,G4,G5,G6,G7,G8)),
    save_games('games_db.pl').

% Сохранение базы в файл
save_games(File) :-
    open(File, write, Stream),
    forall(game(Name,G1,G2,G3,G4,G5,G6,G7,G8),
           format(Stream, "game('~w', '~w', '~w', '~w', '~w', '~w', '~w', '~w', '~w').~n",
                  [Name,G1,G2,G3,G4,G5,G6,G7,G8])),
    close(Stream).
