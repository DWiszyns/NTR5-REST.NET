import React from 'react';
import axios from 'axios';
import NoteEdit from '../components/NoteEdit/NoteEdit'

const API = 'https://localhost:5001';

const NoteContainer = props => {
    const [note, setNote] = React.useState();
    const URL = `${API}/notes/${props.match.params.idnote}`;
    let respon={}
    React.useEffect(() => {
        axios
            .get(URL)
            .then(res => respon=res.data)
            .then(({ data, status }) => {
                console.log(data);
                setNote(data);
            })
            .catch(err => console.log(err.message));
        console.log(respon);
    }, []);

    return (
        <div>
            <h1>Edit Note</h1>
            {note && <NoteEdit
                idnote={note.noteID}
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
