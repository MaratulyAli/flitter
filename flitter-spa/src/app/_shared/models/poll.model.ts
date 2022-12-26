export interface IPoll {
    id: number;
    expiresAt: string | null;
    options: IOption[];
    postId: number;
    userId: string;
}

export interface IOption {
    id: number;
    text: string;
    votesCount: number;
    voted: boolean;
    pollId: number;
}

export interface IPollCreate {
    expiresAt: string;
    options: IOptionCreate[];
}

export interface IOptionCreate {
    id: number | null;
    text: string;
}

export interface IOptionVoteCreate {
    optionId: number;
    pollId: number;
    postId: number;
}