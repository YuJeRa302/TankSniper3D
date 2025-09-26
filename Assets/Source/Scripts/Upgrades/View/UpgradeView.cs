using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Game.Scripts.States;
using Assets.Source.Scripts.Grid;
using Assets.Source.Scripts.Models;
using Assets.Source.Scripts.ScriptableObjects;
using Assets.Source.Scripts.Views;
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

        [SerializeField] private TMP_Text _nameTankText;
        [SerializeField] private TMP_Text _levelTankText;
        [Space(20)]
        [SerializeField] private Button _createTankCardButton;
        [SerializeField] private Button _createPatternCardButton;
        [SerializeField] private Button _createDecalCardButton;
        [SerializeField] private Button _createHeroCardButton;
        [Space(20)]
        [SerializeField] private Button _backButton;
        [Space(20)]
        [SerializeField] private DecorationCardView _decorationCardView;
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
        [SerializeField] private PlacePreviewView _placePreviewView;

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

        private void OnDestroy()
        {
            RemoveListeners();
            ClearSelectionButtons();
        }

        public void Initialize(UpgradeModel upgradeModel, UpgradeConfig upgradeConfig)
        {
            _upgradeModel = upgradeModel;
            _upgradeConfig = upgradeConfig;
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
            Message.Publish(new M_CloseUpgrade());
        }

        private void OnOpen()
        {
            ChangeSetActiveObjects(gameObject, _sceneGameObjects, true);
            CreateSelectionButtons();
            TankState tankState = _upgradeModel.GetTankStateByEquip();
            CreateTank(tankState, _typeHeroSpawn);
            CreateHero(tankState.HeroId, _typeHeroSpawn);
            CreateTankButtons();
        }

        private void OnRewardTaked() // возможно нужно будет убрать TypeCard
        {
            Message.Publish(new M_UpgradeUnlocked(_currentCardIndex, _currentTypeCard));
        }

        private void OnDecorationSelected(DecorationCardView decorationCardView)
        {
            Message.Publish(new M_DeselectCards(decorationCardView.DecorationData.Id));
            _upgradeModel.SelectDecoration(decorationCardView.DecorationState);
        }

        private void OnHeroSelected(HeroCardView heroCardView)
        {
            Message.Publish(new M_DeselectCards(heroCardView.HeroData.Id));
            CreateHero(heroCardView.HeroState.Id, _typeHeroSpawn);
            _upgradeModel.SelectHero(heroCardView.HeroState);
        }

        private void OnTankSelected(TankCardView tankCardView)
        {
            Message.Publish(new M_DeselectCards(tankCardView.TankData.Id));
            _upgradeModel.SelectTank(tankCardView.TankState);
        }

        private void OnBuyButtonClicked(int id, TypeCard typeCard)
        {
            _currentCardIndex = id;
            _currentTypeCard = typeCard;
        }

        private void OnDecalButtonClicked(SelectionButtonView selectionButtonView)
        {
            CreateDecarationButtons(_upgradeConfig.DecalDatas);
            _placePreviewView.SetDecalRotationView();
            Message.Publish(new M_DeselectButtons(selectionButtonView));
        }

        private void OnPatternButtonClicked(SelectionButtonView selectionButtonView)
        {
            CreateDecarationButtons(_upgradeConfig.PatternDatas);
            _placePreviewView.ResetRotation();
            Message.Publish(new M_DeselectButtons(selectionButtonView));
        }

        private void OnHeroButtonClicked(SelectionButtonView selectionButtonView)
        {
            CreateHeroButtons();
            _placePreviewView.SetHeroRotationView();
            Message.Publish(new M_DeselectButtons(selectionButtonView));
        }

        private void OnTankButtonClicked(SelectionButtonView selectionButtonView)
        {
            CreateTankButtons();
            _placePreviewView.ResetRotation();
            Message.Publish(new M_DeselectButtons(selectionButtonView));
        }

        private void CreateTank(TankState tankState, TypeHeroSpawn typeHeroSpawn)
        {
            TankData tankData = _upgradeConfig.GetTankDataById(tankState.Id);
            _tankView = Instantiate(tankData.MainTankView, _tankSpawnPoint);

            _tankView.Initialize(
                tankData,
                _upgradeConfig.GetDecalDataById(tankState.DecalId),
                _upgradeConfig.GetPatternDataById(tankState.PatternId),
                _upgradeConfig.GetHeroDataById(tankState.HeroId),
                typeHeroSpawn);
        }

        private void CreateHero(int id, TypeHeroSpawn typeHeroSpawn)
        {
            HeroData heroData = _upgradeConfig.GetHeroDataById(id);
            _currentHeroView = Instantiate(heroData.HeroView, _heroSpawnPoint);
            _currentHeroView.Initialize(heroData, typeHeroSpawn);
        }

        private void CreateDecarationButtons(List<DecorationData> decorationDatas)
        {
            ClearAllButtons();

            foreach (DecorationData decorationData in decorationDatas)
            {
                DecorationCardView view = Instantiate(_decorationCardView, _cardsContainer);
                _decorationCardViews.Add(view);
                DecorationState decorationState = _upgradeModel.GetDecorationState(decorationData);
                view.Initialize(decorationData, decorationState);
                view.DecorationSelected += OnDecorationSelected;
                view.BuyButtonClicked += OnBuyButtonClicked;
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
                view.Initialize(heroData, heroState);
                view.Selected += OnHeroSelected;
                view.BuyButtonClicked += OnBuyButtonClicked;
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
                view.Selected += OnTankSelected;
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

        private void ClearDecarationButtons()
        {
            if (_decorationCardViews.Count == 0)
                return;

            foreach (DecorationCardView view in _decorationCardViews)
            {
                view.DecorationSelected -= OnDecorationSelected;
                view.BuyButtonClicked -= OnBuyButtonClicked;
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
                view.Selected -= OnHeroSelected;
                view.BuyButtonClicked -= OnBuyButtonClicked;
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
                view.Selected += OnTankSelected;
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
    }
}