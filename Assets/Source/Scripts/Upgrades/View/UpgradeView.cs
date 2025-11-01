using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Game.Scripts.States;
using Assets.Source.Scripts.Grid;
using Assets.Source.Scripts.Models;
using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Sound;
using Assets.Source.Scripts.Views;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Scripts.Upgrades
{
    public class UpgradeView : BaseView
    {
        public static readonly IMessageBroker Message = new MessageBroker();

        private readonly TypeHeroSpawn _typeHeroSpawn = TypeHeroSpawn.Upgrade;
        private readonly float _tweenAnimationDuration = 0.2f;
        private readonly float _tweenAnimationScaler = 0.8f;
        private readonly float _tweenMultiplier = 2f;
        private readonly float _focalLengthValue = 67.17691f;

        [SerializeField] private TMP_Text _moneyText;
        [SerializeField] private TMP_Text _nameTankText;
        [SerializeField] private TMP_Text _levelTankText;
        [SerializeField] private TMP_Text _nameButtonText;
        [Space(20)]
        [SerializeField] private Button _backButton;
        [Space(20)]
        [SerializeField] private DecorationCardView _patternCardView;
        [SerializeField] private DecorationCardView _decalCardView;
        [SerializeField] private HeroCardView _heroCardView;
        [SerializeField] private TankCardView _tankCardView;
        [SerializeField] private SelectionButtonView _selectionButtonView;
        [Space(20)]
        [SerializeField] private Transform _tankSpawnPoint;
        [SerializeField] private Transform _heroSpawnPoint;
        [SerializeField] private Transform _cardsContainer;
        [SerializeField] private Transform _selectionButtonContainer;
        [Space(20)]
        [SerializeField] private GameObject _sceneGameObjects;
        [Space(20)]
        [SerializeField] private ScrollRect _scrollRectButtons;
        [Space(20)]
        [SerializeField] private PlacePreviewView _placePreviewView;
        [Space(20)]
        [SerializeField] private Vector3 _tankSpawnRotation;
        [SerializeField] private Vector3 _tankSpawnPosition;

        private Coroutine _coroutineTankAnimation;
        private CompositeDisposable _disposables = new();
        private List<DecorationCardView> _decorationCardViews = new();
        private List<HeroCardView> _heroCardViews = new();
        private List<TankCardView> _tankCardViews = new();
        private List<SelectionButtonView> _selectionButtonViews = new();
        private HeroView _currentHeroView;
        private TankView _tankView;
        private int _currentCardIndex;
        private TypeCard _currentTypeCard;
        private UpgradeModel _upgradeModel;
        private UpgradeConfig _upgradeConfig;
        private AudioPlayer _audioPlayer;
        private Vector3 _defaultTankScale;
        private Vector3 _defaultHeroScale;

        private void OnDestroy()
        {
            RemoveListeners();
            ClearSelectionButtons();
        }

        public void Initialize(UpgradeModel upgradeModel, UpgradeConfig upgradeConfig, AudioPlayer audioPlayer)
        {
            _upgradeModel = upgradeModel;
            _upgradeConfig = upgradeConfig;
            _audioPlayer = audioPlayer;
            AddListeners();
        }

        private void AddListeners()
        {
            _backButton.onClick.AddListener(Close);

            GridView.Message
                .Receive<M_CloseGrid>()
                .Subscribe(m => OnOpen())
                .AddTo(_disposables);
        }

        private void RemoveListeners()
        {
            _backButton.onClick.RemoveListener(Close);
            _disposables?.Dispose();
        }

        private void Close()
        {
            ChangeSetActiveObjects(gameObject, _sceneGameObjects, false);
            ClearSelectionButtons();
            Message.Publish(new M_CloseUpgrade());
        }

        private void OnOpen()
        {
            ChangeSetActiveObjects(gameObject, _sceneGameObjects, true);
            CreateSelectionButtons();
            _moneyText.text = _upgradeModel.GetMoney().ToString();
            TankState tankState = _upgradeModel.GetTankStateByEquip();
            _upgradeModel.SelectTank(tankState);
            CreateTankEntities(tankState, _typeHeroSpawn);
            SelectTankButton();
            UpdateTankDescription();
            Camera.main.focalLength = _focalLengthValue;
        }

        private void OnDecorationCardSelected(DecorationCardView decorationCardView)
        {
            Message.Publish(new M_DeselectCards(decorationCardView.DecorationData.Id));
            _upgradeModel.SelectDecoration(decorationCardView.DecorationState);
            UpdateTankEntities();
            AnimateSelectionObject(_tankView.gameObject, _defaultTankScale);
        }

        private void OnHeroCardSelected(HeroCardView heroCardView)
        {
            Message.Publish(new M_DeselectCards(heroCardView.HeroData.Id));
            CreateHero(heroCardView.HeroState.Id, _typeHeroSpawn);
            _upgradeModel.SelectHero(heroCardView.HeroState);
            _defaultHeroScale = _currentHeroView.gameObject.transform.localScale;
            AnimateSelectionObject(_currentHeroView.gameObject, _defaultHeroScale);
        }

        private void OnTankCardSelected(TankCardView tankCardView)
        {
            Message.Publish(new M_DeselectCards(tankCardView.TankData.Id));
            _upgradeModel.SelectTank(tankCardView.TankState);
            CreateTankEntities(tankCardView.TankState, _typeHeroSpawn);
            UpdateTankDescription();
            AnimateSelectionObject(_tankView.gameObject, _defaultTankScale);
        }

        private void OnDecalButtonClicked(SelectionButtonView selectionButtonView)
        {
            CreateDecarationButtons(_upgradeConfig.DecalDatas, _decalCardView);
            _placePreviewView.SetDecalRotationView();
            SelectButton(selectionButtonView);
        }

        private void OnPatternButtonClicked(SelectionButtonView selectionButtonView)
        {
            CreateDecarationButtons(_upgradeConfig.PatternDatas, _patternCardView);
            _placePreviewView.ResetRotation();
            SelectButton(selectionButtonView);
        }

        private void OnHeroButtonClicked(SelectionButtonView selectionButtonView)
        {
            CreateHeroButtons();
            _placePreviewView.SetHeroRotationView();
            SelectButton(selectionButtonView);
        }

        private void OnTankButtonClicked(SelectionButtonView selectionButtonView)
        {
            CreateTankButtons();
            _placePreviewView.ResetRotation();
            SelectButton(selectionButtonView);
        }

        private void OnBuyButtonClicked(int id, TypeCard typeCard)
        {
            _currentCardIndex = id;
            _currentTypeCard = typeCard;
            _upgradeModel.UnlockByReward(_currentCardIndex, _currentTypeCard);
            Message.Publish(new M_UpgradeUnlocked(_currentCardIndex, _currentTypeCard));
        }

        private void UpdateTankDescription()
        {
            _levelTankText.text = "Уровень " + _tankView.Level;
            _nameTankText.text = _tankView.Name;
        }

        private void UpdateTankEntities()
        {
            _tankView.UpdateTankEntities(
                _upgradeConfig.GetDecalDataById(_tankView.TankState.DecalId),
                _upgradeConfig.GetPatternDataById(_tankView.TankState.PatternId),
                _upgradeConfig.GetHeroDataById(_tankView.TankState.HeroId),
                _typeHeroSpawn);
        }

        private void SelectButton(SelectionButtonView selectionButtonView)
        {
            _scrollRectButtons.horizontalNormalizedPosition = 0;
            Message.Publish(new M_DeselectButtons(selectionButtonView));
            _nameButtonText.text = selectionButtonView.SelectionButtonData.Name;
        }

        private void SelectTankButton()
        {
            if (_selectionButtonViews.Count > 0)
            {
                foreach (var button in _selectionButtonViews)
                {
                    if (button.SelectionButtonData.TypeCard == TypeCard.Tank)
                        OnTankButtonClicked(button);
                }
            }
        }

        private void CreateTankEntities(TankState tankState, TypeHeroSpawn typeHeroSpawn)
        {
            ClearTankView();
            TankData tankData = _upgradeConfig.GetTankDataById(tankState.Id);

            _tankView = Instantiate(
                tankData.MainTankView,
                new Vector3(
                    _tankSpawnPosition.x,
                    _tankSpawnPosition.y,
                    _tankSpawnPosition.z),
                Quaternion.identity,
                _tankSpawnPoint);

            _tankView.transform.eulerAngles = new Vector3(
                _tankSpawnRotation.x,
                _tankSpawnRotation.y,
                _tankSpawnRotation.z);

            _tankView.Initialize(
                tankState,
                tankData,
                _upgradeConfig.GetDecalDataById(tankState.DecalId),
                _upgradeConfig.GetPatternDataById(tankState.PatternId),
                _upgradeConfig.GetHeroDataById(tankState.HeroId),
                _audioPlayer,
                typeHeroSpawn);

            _defaultTankScale = _tankView.transform.localScale;
            CreateHero(tankState.HeroId, _typeHeroSpawn);
        }

        private void CreateHero(int id, TypeHeroSpawn typeHeroSpawn)
        {
            ClearHeroView();
            HeroData heroData = _upgradeConfig.GetHeroDataById(id);

            if (heroData == null)
                return;

            _currentHeroView = Instantiate(heroData.HeroView, _heroSpawnPoint);
            _currentHeroView.Initialize(heroData, typeHeroSpawn);
        }

        private void CreateDecarationButtons(List<DecorationData> decorationDatas, DecorationCardView decorationCardView)
        {
            ClearAllButtons();

            foreach (DecorationData decorationData in decorationDatas)
            {
                DecorationCardView view = Instantiate(decorationCardView, _cardsContainer);
                _decorationCardViews.Add(view);
                DecorationState decorationState = _upgradeModel.GetDecorationState(decorationData);
                view.Initialize(decorationData, decorationState, _upgradeModel.TankState);
                view.DecorationSelected += OnDecorationCardSelected;
                view.UpgradeButtonAdsWaiter.AdsOpened += OnBuyButtonClicked;
            }
        }

        private void CreateHeroButtons()
        {
            ClearAllButtons();

            foreach (HeroData heroData in _upgradeConfig.HeroDatas)
            {
                HeroCardView view = Instantiate(_heroCardView, _cardsContainer);
                _heroCardViews.Add(view);
                HeroState heroState = _upgradeModel.GetHeroState(heroData);
                view.Initialize(heroData, heroState, _upgradeModel.TankState);
                view.Selected += OnHeroCardSelected;
                view.UpgradeButtonAdsWaiter.AdsOpened += OnBuyButtonClicked;
            }
        }

        private void CreateTankButtons()
        {
            ClearAllButtons();

            foreach (TankData tankData in _upgradeConfig.TankDatas)
            {
                TankCardView view = Instantiate(_tankCardView, _cardsContainer);
                _tankCardViews.Add(view);
                TankState tankState = _upgradeModel.GetTankState(tankData);
                view.Initialize(tankData, tankState);
                view.Selected += OnTankCardSelected;
            }
        }

        private void CreateSelectionButtons()
        {
            foreach (SelectionButtonData selectionButtonData in _upgradeConfig.SelectionButtonDatas)
            {
                SelectionButtonView view = Instantiate(_selectionButtonView, _selectionButtonContainer);
                _selectionButtonViews.Add(view);
                view.Initialize(selectionButtonData);
                view.IUseButtonStrategy.TankButtonClicked += OnTankButtonClicked;
                view.IUseButtonStrategy.PatternButtonClicked += OnPatternButtonClicked;
                view.IUseButtonStrategy.DecalButtonClicked += OnDecalButtonClicked;
                view.IUseButtonStrategy.HeroButtonClicked += OnHeroButtonClicked;
            }
        }

        private void ClearTankView()
        {
            if (_tankView != null)
                Destroy(_tankView.gameObject);
        }

        private void ClearHeroView()
        {
            if (_currentHeroView != null)
                Destroy(_currentHeroView.gameObject);
        }

        private void ClearDecarationButtons()
        {
            if (_decorationCardViews.Count == 0)
                return;

            foreach (DecorationCardView view in _decorationCardViews)
            {
                view.DecorationSelected -= OnDecorationCardSelected;
                view.UpgradeButtonAdsWaiter.AdsOpened -= OnBuyButtonClicked;
                Destroy(view.gameObject);
            }

            _decorationCardViews.Clear();
        }

        private void ClearHeroButtons()
        {
            if (_heroCardViews.Count == 0)
                return;

            foreach (HeroCardView view in _heroCardViews)
            {
                view.Selected -= OnHeroCardSelected;
                view.UpgradeButtonAdsWaiter.AdsOpened -= OnBuyButtonClicked;
                Destroy(view.gameObject);
            }

            _heroCardViews.Clear();
        }

        private void ClearTankButtons()
        {
            if (_tankCardViews.Count == 0)
                return;

            foreach (TankCardView view in _tankCardViews)
            {
                view.Selected += OnTankCardSelected;
                Destroy(view.gameObject);
            }

            _tankCardViews.Clear();
        }

        private void ClearSelectionButtons()
        {
            if (_selectionButtonViews.Count == 0)
                return;

            foreach (SelectionButtonView view in _selectionButtonViews)
            {
                view.IUseButtonStrategy.TankButtonClicked -= OnTankButtonClicked;
                view.IUseButtonStrategy.PatternButtonClicked -= OnPatternButtonClicked;
                view.IUseButtonStrategy.DecalButtonClicked -= OnDecalButtonClicked;
                view.IUseButtonStrategy.HeroButtonClicked -= OnHeroButtonClicked;
                Destroy(view.gameObject);
            }

            _selectionButtonViews.Clear();
        }

        private void ClearAllButtons()
        {
            ClearDecarationButtons();
            ClearHeroButtons();
            ClearTankButtons();
        }

        private void AnimateSelectionObject(GameObject gameObject, Vector3 defaultScale)
        {
            if (_coroutineTankAnimation != null)
                StopCoroutine(_coroutineTankAnimation);

            _coroutineTankAnimation = StartCoroutine(SetAnimation(gameObject, defaultScale));
        }

        private IEnumerator SetAnimation(GameObject gameObject, Vector3 defaultScale)
        {
            Transform transform = gameObject.transform;
            Vector3 startScale = defaultScale;
            Vector3 targetScale = new(startScale.x, startScale.y * _tweenAnimationScaler, startScale.z);

            transform.DOKill();

            Sequence seq = DOTween.Sequence();

            seq.Append(
                transform.
                DOScale(targetScale, _tweenAnimationDuration / _tweenMultiplier).
                SetEase(Ease.OutQuad));

            seq.Append(
                transform.
                DOScale(startScale, _tweenAnimationDuration / _tweenMultiplier).
                SetEase(Ease.OutBounce));

            seq.SetLink(gameObject);
            seq.Play();

            yield return new WaitForSeconds(_tweenAnimationDuration);
            _coroutineTankAnimation = null;
        }
    }
}