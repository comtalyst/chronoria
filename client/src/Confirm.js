import React, {useState, useEffect} from 'react';
import 'flowbite';
import Modal from 'flowbite/src/components/modal';
import _ from 'lodash';
import axios from 'axios';

import config from './config.json';
import dialogues from './dialogues.json';
import { useParams } from 'react-router-dom';

function Confirm() {
  const [currentSuccess, setCurrentSuccess]         = useState('');
  const [currentError, setCurrentError]             = useState('');
  const [confirmModal, setConfirmModal]            = useState(null);

  useEffect(() => {
    if(confirmModal != null){
      return;
    }
    const modal = new Modal(document.getElementById('confirm'));
    modal.show();
    setConfirmModal(modal);
  })

  let params = useParams();
  (async (id) => {
    try{
      await axios.get(config.urls.webAPI + '/confirm', {id});
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
      return;
    }
    setCurrentSuccess(dialogues.success.CONFIRM);
  })(params.id);

  return (
    <div className='flex flex-col min-h-screen bg-light_bg text-light_text text-lg'>
      <div id='confirm' aria-hidden='true' className='hidden overflow-y-auto overflow-x-hidden fixed top-0 right-0 left-0 z-50 w-full md:inset-0 h-modal md:h-full'>
        <div className='relative p-4 w-full max-w-2xl h-auto'>
          <div className='relative bg-white rounded-lg shadow'>
            <div className='flex justify-between items-start p-5 rounded-t-lg border-b'>
              <h3 className='text-xl font-semibold '>
                Confirmation
              </h3>
            </div>
            <div className='p-6'>
              { (currentError || currentSuccess)? (<div/>) : (
                  <div className='p-4 mx-4 mb-4 text-sm text-blue-700 bg-blue-100 rounded-lg' role='alert'>
                    <span className='font-semibold'>Please wait</span>&nbsp;&nbsp;&nbsp;&nbsp;{dialogues.info.PROCESSING}
                  </div>
                )
              }
              { (currentSuccess === '')? (<div/>) : (
                  <div className='p-4 mx-4 mb-4 text-sm text-green-700 bg-green-100 rounded-lg' role='alert'>
                    <span className='font-semibold'>Success!</span>&nbsp;&nbsp;&nbsp;&nbsp;{currentSuccess}
                  </div>
                )
              }
              { (currentError === '')? (<div/>) : (
                  <div className='p-4 mx-4 mb-4 text-sm text-red-700 bg-red-100 rounded-lg' role='alert'>
                    <span className='font-semibold'>Error!</span>&nbsp;&nbsp;&nbsp;&nbsp;{currentError}
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

export default Confirm;