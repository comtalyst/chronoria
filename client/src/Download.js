import React, {useState, useEffect} from 'react';
import 'flowbite';
import Modal from 'flowbite/src/components/modal';
import axios from 'axios';

import config from './config.json';
import dialogues from './dialogues.json';
import { useParams, useNavigate } from 'react-router-dom';

function Download() {
  const navigate = useNavigate();
  let params = useParams();
  const id = params.id;
  const [recipientEmailRaw, setRecipientEmailRaw]   = useState('');
  const [submitting, setSubmitting]                 = useState(false);
  const [currentSuccess, setCurrentSuccess]         = useState('');
  const [currentError, setCurrentError]             = useState('');
  const [downloadModal, setDownloadModal]           = useState(null);

  // Submission service
  const trueSubmit = async (e) => {
    e.preventDefault();
    const recipientEmail = recipientEmailRaw.trim();
    const resp = await axios.get(config.urls.webAPI + '/downloadlink', {params: {id, recipientEmail}}).catch((e) => {throw new Error((e.response.data != null)? e.response.data:'INTERNAL_SERVER_ERROR')});
    window.location.href = resp.data;
    return true;
  }
  // Submission control
  const submit = async (e) => {
    e.preventDefault();
    setSubmitting(true);
    setCurrentError('');
    setCurrentSuccess('');
    
    // try sending request
    try{
      const res = await trueSubmit(e);
      if(!res){
        setSubmitting(false);
        return false;
      }
    } catch (e){
      const alertMessage = ((errorMessage) => {
        if(dialogues.error[errorMessage] != null){
          return dialogues.error[errorMessage];
        }
        else{
          return errorMessage;
        }
      })(e.message);
      setCurrentError(alertMessage);
      setSubmitting(false);
      return false;
    }
    // complete
    setCurrentSuccess(dialogues.success.DOWNLOAD);
    setSubmitting(false);
    return true;
  }

  useEffect(() => {
    if(downloadModal != null){
      return;
    }
    const modal = new Modal(document.getElementById('download'));
    modal.show();
    setDownloadModal(modal);
  }, [downloadModal]);

  return (
    <div className='flex flex-col min-h-screen bg-light_bg text-light_text text-lg'>
      <div id='download' aria-hidden='true' className='hidden overflow-y-auto overflow-x-hidden fixed top-0 right-0 left-0 z-50 w-full md:inset-0 h-modal md:h-full'>
        <div className='relative p-4 w-full max-w-2xl h-auto'>
          <div className='relative bg-white rounded-lg shadow'>
            <div className='flex justify-between items-start p-5 rounded-t-lg border-b'>
              <h3 className='text-xl font-semibold '>
                Download attachments
              </h3>
            </div>
            <div className='flex flex-col p-6'>
              { (!submitting)? (<div/>) : (
                  <div className='w-full p-4 mb-4 text-sm text-blue-700 bg-blue-100 rounded-lg' role='alert'>
                    <span className='font-semibold'>Please wait</span>&nbsp;&nbsp;&nbsp;&nbsp;{dialogues.info.PROCESSING}
                  </div>
                )
              }
              { (currentSuccess === '')? (<div/>) : (
                  <div className='w-full p-4 mb-4 text-sm text-green-700 bg-green-100 rounded-lg' role='alert'>
                    <span className='font-semibold'>Success!</span>&nbsp;&nbsp;&nbsp;&nbsp;{currentSuccess}
                  </div>
                )
              }
              { (currentError === '')? (<div/>) : (
                  <div className='w-full p-4 mb-4 text-sm text-red-700 bg-red-100 rounded-lg' role='alert'>
                    <span className='font-semibold'>Error!</span>&nbsp;&nbsp;&nbsp;&nbsp;{currentError}
                  </div>
                )
              }
              { (currentSuccess !== '')? (<div/>) : (
                  <div>
                    <p className='text-base leading-relaxed text-light_text_l mb-4'>
                      Please verify the email of the recipient of the letter to continue.
                    </p>
                    <form onSubmit={submit} className='flex space-x-4'>
                      <input id='recipientEmail' type='email' placeholder='they@example.com' required maxLength={config.policies.maxEmailCh} onChange={(e) => setRecipientEmailRaw(e.target.value)}
                        className='border border-light_border  rounded-lg focus:ring-light_hl focus:border-light_hl w-full'/>
                      {
                        (!submitting)? (
                          <button className='bg-light_hl w-fit px-5 py-2 text-light_hl_subtext font-bold text-2xl rounded-lg
                                        hover:bg-light_hl_l hover:scale-105
                                        transition-all duration-200'
                                  type='submit' disabled={submitting}>
                            Submit
                          </button>
                        ) : (
                          <button className='bg-light_disabled w-fit px-5 py-2 text-light_hl_subtext font-bold text-2xl rounded-lg
                                        transition-all duration-200'
                                  type='submit' disabled>
                            Processing...
                          </button>
                        )
                      }
                    </form>
                  </div>
                )
              }
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}

export default Download;