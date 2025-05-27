:- dynamic game/2.
:- dynamic question/3.

:- consult('games_db.pl').
:- consult('questions_db.pl').

% Возвращает вопрос по номеру
ask_question(Num, Q, O) :-
    question(Num, Q, O).

% Проверка, совпадает ли список как префикс
prefix([], _).
prefix([H|T], [H2|T2]) :-
    H == H2,
    prefix(T, T2).

% Находим подходящие игры
possible_games(Answers, Games) :-
    findall(Name, (game(Name, FullAnswers),
                   prefix(Answers, FullAnswers)), Games).

% Возвращает следующий вопрос
next_question(Answers, Question, Options) :-
    length(Answers, Len),
    NextQ is Len + 1,
    ask_question(NextQ, Question, Options).

% Основной предикат хода
next_step(Answers, Response, Done) :-
    possible_games(Answers, Games),
    ( Games = [] ->
        Response = no_match,
        Done = true
    ; Games = [OnlyGame] ->
        Response = game(OnlyGame),
        Done = true
    ; next_question(Answers, Q, O) ->
        Response = question(Q, O),
        Done = false
    ; Response = multiple_matches,
      Done = true
    ).

questions_count(Count) :-
    findall(_, question(_,_,_), Qs),
    length(Qs, Count).

% Добавление игры
add_and_save_game(Name, Answers) :-
    assertz(game(Name, Answers)),
    save_games('games_db.pl').

% Обновление существующей игры
update_game(Name, Answers) :-
    retract(game(Name, _)),
    assertz(game(Name, Answers)),
    save_games('games_db.pl').

% Добавление нового вопроса
add_question(Text, Options) :-
    findall(_, question(_,_,_), Qs),
    length(Qs, N),
    Id is N + 1,
    assertz(question(Id, Text, Options)),
    save_questions('questions_db.pl'),
    write(Id).

% Сохраняем вопросы
save_questions(File) :-
    open(File, write, Stream),
    forall(question(Id, Q, O),
        format(Stream, "question(~w, '~w', ~w).~n", [Id, Q, O])),
    close(Stream).

% Сохраняем игры
save_games(File) :-
    open(File, write, Stream),
    forall(game(Name, Answers),
        format(Stream, "game('~w', ~w).~n", [Name, Answers])),
    close(Stream).

% Получить список всех игр
list_games :-
    findall(Name, game(Name, _), Games),
    write(Games).

% Получить список всех вопросов
list_questions :-
    forall(question(Id, Text, Options),
        format("~w|||~w|||~w~n", [Id, Text, Options])).

count_possible_games(Answers, Count) :-
    possible_games(Answers, Games),
    length(Games, Count).