import React from 'react';
import axios from 'axios';
import NoteEdit from '../components/NoteEdit/NoteEdit'

const API = 'http://localhost:5000/api';

const NoteContainer = props => {
    const [note, setNote] = React.useState();
    React.useEffect(() => {
        axios
            .get(`${API}/notes/${props.match.params.title}`)
            .then(res => res.data)
            .then(({ data }) => {
                console.log(data);
                setNote(data);
            })
            .catch(err => console.log(err.message));
    }, []);

    return (
        <div>
            <h1>Edit Note</h1>
            {note && <NoteEdit
                title={note.title}
                content={note.text}
                noteCategories={note.noteCategories}
                markdown={note.markdown}
                date={note.date}
            />}
        </div>
    );
};

export default NoteContainer;
